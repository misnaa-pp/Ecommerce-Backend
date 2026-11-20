
using KidsWearAPI.Data;
using KidsWearAPI.DTOs;
using KidsWearAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KidsWearAPI.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly AppDbContext _context;

        public AdminDashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardResponse> GetDashboard()
        {
            var dashboard = new AdminDashboardResponse
            {
                TotalProducts = await _context.Products.CountAsync(),

                TotalCustomers = await _context.Users
                    .CountAsync(u => u.Role == "User"),

                TotalOrders = await _context.Orders.CountAsync(),

                PendingOrders = await _context.Orders
                    .CountAsync(o => o.OrderStatus == "Pending"),

                TotalRevenue = await _context.Orders
                    .Where(o => o.OrderStatus != "Cancelled")
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0,

                LowStockProducts = await _context.Products
                    .CountAsync(p => p.Stock > 0 && p.Stock <= 5),

                OutOfStockProducts = await _context.Products
                    .CountAsync(p => p.Stock == 0)
            };

           
            dashboard.CategoryData = await _context.Products
                .GroupBy(p => p.Category)
                .Select(g => new CategorySummary
                {
                    Name = g.Key,
                    Value = g.Count()
                })
                .ToListAsync();

           
            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-5);

            var monthlyData = await _context.Orders
                .Where(o => o.OrderDate >= new DateTime(
                    sixMonthsAgo.Year, sixMonthsAgo.Month, 1)
                    && o.OrderStatus != "Cancelled")
                .GroupBy(o => new
                {
                    o.OrderDate.Year,
                    o.OrderDate.Month
                })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .ToListAsync();

            dashboard.MonthlySales = monthlyData
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .Select(m => new MonthlySalesSummary
                {
                    Month = new DateTime(m.Year, m.Month, 1)
                        .ToString("MMM"),
                    Revenue = m.Revenue
                })
                .ToList();

            
            dashboard.RecentOrders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .Select(o => new RecentOrderSummary
                {
                    Id = o.OrderId,
                    CustomerName = o.User.Name,
                    CreatedAt = o.OrderDate,
                    Total = o.TotalAmount,
                    Status = o.OrderStatus
                })
                .ToListAsync();

            return dashboard;
        }
    }
}
