
using KidsWearAPI.Configuration;
using KidsWearAPI.Data;
using KidsWearAPI.DTOs.Payment;
using KidsWearAPI.Interfaces;
using KidsWearAPI.Models;
using PaymentModel = KidsWearAPI.Models.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;

namespace KidsWearAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly RazorpaySettings _settings;

        public PaymentService(
            AppDbContext context,
            IOptions<RazorpaySettings> settings)
        {
            _context = context;
            _settings = settings.Value;
        }

        public async Task<CreateRazorpayOrderResponse?> CreateRazorpayOrder(
            int orderId,
            int userId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o =>
                    o.OrderId == orderId &&
                    o.UserId == userId);

            if (order == null)
                return null;

            if (order.PaymentMethod != "OnlinePayment")
                throw new InvalidOperationException(
                    "This order is not configured for online payment.");

            if (order.PaymentStatus != "Pending")
                throw new InvalidOperationException(
                    "This order cannot be paid.");

            if (string.IsNullOrWhiteSpace(_settings.KeyId) ||
                string.IsNullOrWhiteSpace(_settings.KeySecret))
                throw new InvalidOperationException(
                    "Razorpay credentials are not configured.");

            // Reuse an existing pending Razorpay order if available
            var existingPayment = await _context.Payments
                .FirstOrDefaultAsync(p =>
                    p.OrderId == orderId &&
                    p.PaymentStatus == "Pending");

            if (existingPayment?.RazorpayOrderId != null)
            {
                return new CreateRazorpayOrderResponse
                {
                    RazorpayOrderId = existingPayment.RazorpayOrderId,
                    KeyId = _settings.KeyId,
                    Amount = Convert.ToInt32(order.TotalAmount * 100),
                    Currency = "INR"
                };
            }

            var client = new RazorpayClient(
                _settings.KeyId,
                _settings.KeySecret
            );

            int amountInPaise =
                Convert.ToInt32(order.TotalAmount * 100);

            var options = new Dictionary<string, object>
            {
                { "amount", amountInPaise },
                { "currency", "INR" },
                { "receipt", $"kidswear_{order.OrderId}" }
            };

            var razorpayOrder = client.Order.Create(options);

            string razorpayOrderId =
                razorpayOrder["id"].ToString()!;

            var payment = new PaymentModel
            {
                OrderId = orderId,
                PaymentMethod = "OnlinePayment",
                Amount = order.TotalAmount,
                PaymentStatus = "Pending",
                RazorpayOrderId = razorpayOrderId,
                PaymentDate = DateTime.UtcNow
            };

            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            return new CreateRazorpayOrderResponse
            {
                RazorpayOrderId = razorpayOrderId,
                KeyId = _settings.KeyId,
                Amount = amountInPaise,
                Currency = "INR"
            };
        }


        public async Task<PaymentModel?> VerifyRazorpayPayment(
            VerifyRazorpayPaymentRequest request,
            int userId)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            var order = await _context.Orders
                .FirstOrDefaultAsync(o =>
                    o.OrderId == request.OrderId &&
                    o.UserId == userId);

            if (order == null)
                return null;

            if (order.OrderStatus != "Pending")
                throw new InvalidOperationException(
                    "This order has been cancelled and cannot be paid.");

            await _context.Entry(order)
                .Collection(o => o.OrderItems)
                .LoadAsync();

            var payment = await _context.Payments
                .FirstOrDefaultAsync(p =>
                    p.OrderId == request.OrderId &&
                    p.RazorpayOrderId == request.RazorpayOrderId);

            if (payment == null)
                throw new InvalidOperationException(
                    "Payment record not found.");

            if (payment.PaymentStatus != "Pending" ||
                order.PaymentStatus != "Pending")
                throw new InvalidOperationException(
                    "Payment has already been processed.");

            if (string.IsNullOrWhiteSpace(payment.RazorpayOrderId) ||
                payment.RazorpayOrderId != request.RazorpayOrderId)
                throw new InvalidOperationException(
                    "Invalid Razorpay order.");

            string payload =
                $"{payment.RazorpayOrderId}|{request.RazorpayPaymentId}";

            using var hmac = new HMACSHA256(
                Encoding.UTF8.GetBytes(_settings.KeySecret));

            byte[] expectedSignature = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(payload));

            byte[] receivedSignature;

            try
            {
                receivedSignature =
                    Convert.FromHexString(request.RazorpaySignature);
            }
            catch (FormatException)
            {
                throw new InvalidOperationException(
                    "Invalid payment signature.");
            }

            if (!CryptographicOperations.FixedTimeEquals(
                expectedSignature, receivedSignature))
                throw new InvalidOperationException(
                    "Payment verification failed.");


            var razorpayClient = new RazorpayClient(
                _settings.KeyId,
                _settings.KeySecret
            );

            var razorpayPayment = razorpayClient.Payment.Fetch(
                request.RazorpayPaymentId
            );

            string razorpayStatus =
                razorpayPayment.Attributes["status"].ToString()!;

            string razorpayPaymentOrderId =
                razorpayPayment.Attributes["order_id"].ToString()!;

            string razorpayCurrency =
                razorpayPayment.Attributes["currency"].ToString()!;

            long razorpayAmount = Convert.ToInt64(
                razorpayPayment.Attributes["amount"]
            );

            long expectedAmount = Convert.ToInt64(
                order.TotalAmount * 100
            );

            if (razorpayStatus != "captured")
                throw new InvalidOperationException(
                    "Payment has not been captured yet.");

            if (razorpayPaymentOrderId != payment.RazorpayOrderId)
                throw new InvalidOperationException(
                    "Payment order ID mismatch.");

            if (razorpayAmount != expectedAmount ||
                razorpayCurrency != "INR")
                throw new InvalidOperationException(
                    "Payment amount or currency mismatch.");


            var productIds = order.OrderItems
                .Select(item => item.ProductId)
                .Distinct()
                .ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToListAsync();

            foreach (var item in order.OrderItems)
            {
                var product = products.FirstOrDefault(
                    p => p.ProductId == item.ProductId);

                if (product == null || product.Stock < item.Quantity)
                    throw new InvalidOperationException(
                        $"Insufficient stock for {item.ProductName}.");
            }

            foreach (var item in order.OrderItems)
            {
                var product = products.First(
                    p => p.ProductId == item.ProductId);

                product.Stock -= item.Quantity;
            }

            if (!order.IsBuyNow)
            {
                var cartItems = await _context.Carts
                    .Where(c =>
                        c.UserId == userId &&
                        productIds.Contains(c.ProductId))
                    .ToListAsync();

                _context.Carts.RemoveRange(cartItems);
            }

            payment.PaymentStatus = "Success";
            payment.TransactionId = request.RazorpayPaymentId;
            payment.PaymentDate = DateTime.UtcNow;

            order.PaymentStatus = "Success";

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return payment;
        }

    }
}
