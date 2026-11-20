
using KidsWearAPI.Data;
using KidsWearAPI.DTOs;
using KidsWearAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KidsWearAPI.Services
{
    public class AdminOrderService : IAdminOrderService
    {
        private readonly AppDbContext _context;

        public AdminOrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdminOrderResponse>> GetAllOrders()
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(o => new AdminOrderResponse
            {
                OrderId = o.OrderId,
                CustomerName = o.User.Name,
                CustomerEmail = o.User.Email,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                OrderStatus = o.OrderStatus
            }).ToList();
        }

        public async Task<AdminOrderResponse?> GetOrderById(int id)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return null;

            return new AdminOrderResponse
            {
                OrderId = order.OrderId,
                CustomerName = order.User.Name,
                CustomerEmail = order.User.Email,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus
            };
        }

        public async Task<PaginatedResponse<AdminOrderResponse>> GetFilteredOrders(
            int pageNumber,
            int pageSize,
            string? search,
            string? status)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(o =>
                    o.User.Name.Contains(search) ||
                    o.User.Email.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.Trim();
                query = query.Where(o => o.OrderStatus == status);
            }

            var totalCount = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new AdminOrderResponse
                {
                    OrderId = o.OrderId,
                    CustomerName = o.User.Name,
                    CustomerEmail = o.User.Email,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus
                })
                .ToListAsync();

            return new PaginatedResponse<AdminOrderResponse>
            {
                Items = orders,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(
                    (double)totalCount / pageSize)
            };
        }

        public async Task<AdminOrderDetailsResponse?> GetOrderDetails(int id)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return null;

            return new AdminOrderDetailsResponse
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                OrderStatus = order.OrderStatus,
                PaymentStatus = order.PaymentStatus,
                PaymentMethod = order.PaymentMethod,

                UserId = order.UserId,
                CustomerName = order.User.Name,
                CustomerEmail = order.User.Email,

                FullName = order.FullName,
                Phone = order.Phone,
                AddressLine = order.AddressLine,
                City = order.City,
                State = order.State,
                PostalCode = order.PostalCode,
                Country = order.Country,

                SubTotal = order.SubTotal,
                ShippingCharge = order.ShippingCharge,
                TotalAmount = order.TotalAmount,

                Items = order.OrderItems.Select(item =>
                    new AdminOrderItemResponse
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

        public async Task<string> UpdateOrderStatus(int id, string status)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return "NotFound";

            if (status == "Cancelled" &&
                order.PaymentMethod == "OnlinePayment" &&
                order.PaymentStatus == "Success")
            {
                return "RefundRequired";
            }

            bool isValidTransition = order.OrderStatus switch
            {
                "Pending" =>
                    status == "Processing" || status == "Cancelled",

                "Processing" =>
                    status == "Shipped" || status == "Cancelled",

                "Shipped" =>
                    status == "Delivered",

                "Delivered" => false,

                "Cancelled" => false,

                _ => false
            };

            if (!isValidTransition)
                return "InvalidTransition";

            if (status == "Cancelled" &&
    order.PaymentMethod == "CashOnDelivery")
            {
                var productIds = order.OrderItems
                    .Select(item => item.ProductId)
                    .Distinct()
                    .ToList();

                var products = await _context.Products
                    .Where(p => productIds.Contains(p.ProductId))
                    .ToDictionaryAsync(p => p.ProductId);

                foreach (var item in order.OrderItems)
                {
                    if (products.TryGetValue(item.ProductId, out var product))
                    {
                        product.Stock += item.Quantity;
                    }
                }
            }

            order.OrderStatus = status;

            await _context.SaveChangesAsync();

            return "Success";
        }
    }
}
