using KidsWearAPI.Data;
using KidsWearAPI.DTOs;
using KidsWearAPI.Interfaces;
using KidsWearAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsWearAPI.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly AppDbContext _context;

        public WishlistService(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<bool> AddToWishlist(int userId, int productId)
        {
            var productExists = await _context.Products
                .AnyAsync(p => p.ProductId == productId);

            if (!productExists)
                return false;

            var alreadyExists = await _context.Wishlists
                .AnyAsync(w => w.UserId == userId &&
                               w.ProductId == productId);

            if (alreadyExists)
                return false;

            var wishlist = new Wishlist
            {
                UserId = userId,
                ProductId = productId
            };

            _context.Wishlists.Add(wishlist);

            await _context.SaveChangesAsync();

            return true;
        }

        
        public async Task<IEnumerable<WishlistResponse>> GetWishlist(int userId)
        {

            return await _context.Wishlists
    .Where(w => w.UserId == userId)
    .Select(w => new WishlistResponse
    {
        ProductId = w.ProductId,
        Name = w.Product.Name,
        Price = w.Product.Price,
        OriginalPrice = w.Product.OriginalPrice,
        Stock = w.Product.Stock,
        ImageUrl = w.Product.Images
            .Select(i => i.ImageUrl)
            .FirstOrDefault()
    })
    .ToListAsync();
        }

        
        public async Task<bool> RemoveFromWishlist(int userId, int productId)
        {
            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w =>
                    w.UserId == userId &&
                    w.ProductId == productId);

            if (wishlist == null)
                return false;

            _context.Wishlists.Remove(wishlist);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}