
using KidsWearAPI.Data;
using KidsWearAPI.DTOs;
using KidsWearAPI.Interfaces;
using KidsWearAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsWearAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<OrderResponse?> Checkout(
            int userId, CheckoutRequest request)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a =>
                    a.AddressId == request.AddressId &&
                    a.UserId == userId);

            if (address == null)
                return null;

            var checkoutItems = new List<(Product Product, string? Size, int Quantity)>();

            if (request.ProductId.HasValue)
            {
                if (!request.Quantity.HasValue || request.Quantity.Value <= 0)
                    return null;

                var product = await _context.Products
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p =>
                        p.ProductId == request.ProductId.Value);

                if (product == null || product.Stock < request.Quantity.Value)
                    return null;

                checkoutItems.Add((
                    product,
                    request.Size,
                    request.Quantity.Value
                ));
            }
            else
            {
                var cartItems = await _context.Carts
                    .Include(c => c.Product)
                        .ThenInclude(p => p.Images)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                if (!cartItems.Any())
                    return null;

                foreach (var item in cartItems)
                {
                    if (item.Product == null ||
                        item.Product.Stock < item.Quantity)
                    {
                        return null;
                    }

                    checkoutItems.Add((
                        item.Product,
                        item.Size,
                        item.Quantity
                    ));
                }
            }

            decimal subTotal = checkoutItems.Sum(item =>
                item.Product.Price * item.Quantity);

            decimal shippingCharge = subTotal >= 999 ? 0 : 50;

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                SubTotal = subTotal,
                ShippingCharge = shippingCharge,
                TotalAmount = subTotal + shippingCharge,
                OrderStatus = "Pending",
                PaymentStatus = "Pending",
                PaymentMethod = request.PaymentMethod,
                IsBuyNow = request.ProductId.HasValue,

                FullName = address.FullName,
                Phone = address.Phone,
                AddressLine = address.AddressLine,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country,

                OrderItems = new List<OrderItem>()
            };

            foreach (var item in checkoutItems)
            {
                var product = item.Product;

                var orderItem = new OrderItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.Name,
                    Size = item.Size,
                    ImageUrl = product.Images
                        .FirstOrDefault()?.ImageUrl,
                    UnitPrice = product.Price,
                    Quantity = item.Quantity,
                    SubTotal = product.Price * item.Quantity
                };

                order.OrderItems.Add(orderItem);

                if (request.PaymentMethod == "CashOnDelivery")
                {
                    product.Stock -= item.Quantity;
                }
            }

            _context.Orders.Add(order);

            if (!request.ProductId.HasValue &&
              request.PaymentMethod == "CashOnDelivery")
            {
                var cartItemsToRemove = await _context.Carts
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                _context.Carts.RemoveRange(cartItemsToRemove);
            }

            await _context.SaveChangesAsync();

            return MapOrder(order);
        }


        public async Task<IEnumerable<OrderResponse>> GetMyOrders(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(MapOrder).ToList();
        }

        public async Task<OrderResponse?> GetOrderById(
            int userId, int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o =>
                    o.OrderId == orderId &&
                    o.UserId == userId);

            if (order == null)
                return null;

            return MapOrder(order);
        }

        public async Task<bool> CancelOrder(int userId, int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o =>
                    o.OrderId == orderId &&
                    o.UserId == userId);

            if (order == null)
                return false;

            if (order.OrderStatus != "Pending")
                return false;

            var productIds = order.OrderItems
                .Select(item => item.ProductId)
                .ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToListAsync();

            foreach (var item in order.OrderItems)
            {
                var product = products.FirstOrDefault(
                    p => p.ProductId == item.ProductId);

                if (product != null)
                {
                    product.Stock += item.Quantity;
                }
            }

            order.OrderStatus = "Cancelled";

            await _context.SaveChangesAsync();

            return true;
        }

        private static OrderResponse MapOrder(Order order)
        {
            return new OrderResponse
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                SubTotal = order.SubTotal,
                ShippingCharge = order.ShippingCharge,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                PaymentStatus = order.PaymentStatus,
                PaymentMethod = order.PaymentMethod,

                FullName = order.FullName,
                Phone = order.Phone,
                AddressLine = order.AddressLine,
                City = order.City,
                State = order.State,
                PostalCode = order.PostalCode,
                Country = order.Country,

                Items = order.OrderItems.Select(item =>
                    new OrderItemResponseDto
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Size = item.Size,
                        ImageUrl = item.ImageUrl,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        SubTotal = item.SubTotal
                    }).ToList()
            };
        }
    }
}
