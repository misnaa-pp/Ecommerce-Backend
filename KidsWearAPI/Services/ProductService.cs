
using KidsWearAPI.Data;
using KidsWearAPI.DTOs;
using KidsWearAPI.Interfaces;
using Microsoft.EntityFrameworkCore;



namespace KidsWearAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<PagedResponse<ProductResponse>> GetAllProducts(
            string? search,
            string? category,
            int pageNumber,
            int pageSize)
        {
            
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 50);

            
            var query = _context.Products
                .AsNoTracking()
                .AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Name.Contains(search));
            }

            
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p =>
                    p.Category == category);
            }

           
            int totalCount = await query.CountAsync();

            
            int totalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize
            );

            
            var products = await query
                .OrderBy(p => p.ProductId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductResponse
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Category = p.Category,
                    Price = p.Price,
                    OriginalPrice = p.OriginalPrice,
                    Discount = p.Discount,
                    Description = p.Description,
                    Stock = p.Stock,

                    Sizes = p.Sizes
                        .Select(s => s.Size)
                        .ToList(),

                    Colors = p.Colors
                        .Select(c => c.Color)
                        .ToList(),

                    Images = p.Images
                        .Select(i => i.ImageUrl)
                        .ToList()
                })
                .ToListAsync();

            
            return new PagedResponse<ProductResponse>
            {
                Items = products,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }


        public async Task<ProductResponse?> GetProductById(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Where(p => p.ProductId == id)
                .Select(p => new ProductResponse
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Category = p.Category,
                    Price = p.Price,
                    OriginalPrice = p.OriginalPrice,
                    Discount = p.Discount,
                    Description = p.Description,
                    Stock = p.Stock,

                    Sizes = p.Sizes
                        .Select(s => s.Size)
                        .ToList(),

                    Colors = p.Colors
                        .Select(c => c.Color)
                        .ToList(),

                    Images = p.Images
                        .Select(i => i.ImageUrl)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return product;
        }
    }
}
