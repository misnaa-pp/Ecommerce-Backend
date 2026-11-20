

using KidsWearAPI.Data;
using KidsWearAPI.DTOs;
using KidsWearAPI.Interfaces;
using KidsWearAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsWearAPI.Services
{
    public class AdminProductService : IAdminProductService
    {
        private readonly AppDbContext _context;

        public AdminProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResponse<ProductResponse>> GetAllProducts(
            int pageNumber,
            int pageSize,
            string? search,
            string? category)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 10;

            if (pageSize > 100)
                pageSize = 100;

            var query = _context.Products
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(p =>
                    p.Name.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                category = category.Trim();

                query = query.Where(p =>
                    p.Category == category);
            }

            int totalCount = await query.CountAsync();

            int totalPages = (int)Math.Ceiling(
                (double)totalCount / pageSize
            );

            var products = await query
                .Include(p => p.Sizes)
                .Include(p => p.Colors)
                .Include(p => p.Images)
                .OrderBy(p => p.ProductId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<ProductResponse>
            {
                Items = products.Select(MapToResponse).ToList(),
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
                .Include(p => p.Sizes)
                .Include(p => p.Colors)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return null;

            return MapToResponse(product);
        }

        public async Task<ProductResponse> AddProduct(ProductRequest request)

        {
            ValidateProductPricing(request);
            var product = new Product
            {
                Name = request.Name,
                Category = request.Category,
                Description = request.Description,
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Discount = request.Discount,
                Stock = request.Stock,

                Sizes = request.Sizes.Select(s => new ProductSize
                {
                    Size = s
                }).ToList(),

                Colors = request.Colors.Select(c => new ProductColor
                {
                    Color = c
                }).ToList(),

                Images = request.Images.Select(i => new ProductImage
                {
                    ImageUrl = i
                }).ToList()
            };

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return MapToResponse(product);
        }

        public async Task<ProductResponse?> UpdateProduct(int id,ProductRequest request)
        {
            ValidateProductPricing(request);
            var product = await _context.Products
                .Include(p => p.Sizes)
                .Include(p => p.Colors)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return null;

            product.Name = request.Name;
            product.Category = request.Category;
            product.Description = request.Description;
            product.Price = request.Price;
            product.OriginalPrice = request.OriginalPrice;
            product.Discount = request.Discount;
            product.Stock = request.Stock;

            _context.ProductSizes.RemoveRange(product.Sizes);

            product.Sizes = request.Sizes.Select(s => new ProductSize
            {
                Size = s
            }).ToList();

            _context.ProductColors.RemoveRange(product.Colors);

            product.Colors = request.Colors.Select(c => new ProductColor
            {
                Color = c
            }).ToList();

            _context.ProductImages.RemoveRange(product.Images);

            product.Images = request.Images.Select(i => new ProductImage
            {
                ImageUrl = i
            }).ToList();

            await _context.SaveChangesAsync();

            return MapToResponse(product);
        }
        public async Task<string> DeleteProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Sizes)
                .Include(p => p.Colors)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return "NotFound";

            bool isUsedInOrder = await _context.OrderItems
                .AnyAsync(oi => oi.ProductId == id);

            if (isUsedInOrder)
                return "UsedInOrder";


            bool isInCart = await _context.Carts
                .AnyAsync(c => c.ProductId == id);

            if (isInCart)
                return "InCart";

            bool isInWishlist = await _context.Wishlists
                .AnyAsync(w => w.ProductId == id);

            if (isInWishlist)
                return "InWishlist";


            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return "Success";
        }

        private static ProductResponse MapToResponse(Product product)
        {
            return new ProductResponse
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Category = product.Category,
                Description = product.Description,
                Price = product.Price,
                OriginalPrice = product.OriginalPrice,
                Discount = product.Discount,
                Stock = product.Stock,

                Sizes = product.Sizes
                    .Select(s => s.Size)
                    .ToList(),

                Colors = product.Colors
                    .Select(c => c.Color)
                    .ToList(),

                Images = product.Images
                    .Select(i => i.ImageUrl)
                    .ToList()
            };
        }


        private static void ValidateProductPricing(ProductRequest request)
        {
            if (request.OriginalPrice < request.Price)
            {
                throw new ArgumentException(
                    "Original price cannot be less than selling price."
                );
            }

            decimal expectedDiscount = request.OriginalPrice > 0
                ? Math.Round(
                    ((request.OriginalPrice - request.Price)
                    / request.OriginalPrice) * 100,
                    2
                )
                : 0;

            if (Math.Abs(expectedDiscount - request.Discount) > 0.01m)
            {
                throw new ArgumentException(
                    "Discount percentage does not match the product prices."
                );
            }
        }

    }
}

