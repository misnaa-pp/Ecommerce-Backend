
using KidsWearAPI.Data;
using KidsWearAPI.DTOs;
using KidsWearAPI.Interfaces;
using KidsWearAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsWearAPI.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddToCart(
            int userId, int productId, string size, int quantity)
        {
            if (quantity <= 0 || string.IsNullOrWhiteSpace(size))
                return false;

            var product = await _context.Products
                .Include(p => p.Sizes)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null || product.Stock < quantity)
                return false;

           
            if (!product.Sizes.Any(s => s.Size == size))
                return false;

            var cartItem = await _context.Carts
                .FirstOrDefaultAsync(c =>
                    c.UserId == userId &&
                    c.ProductId == productId &&
                    c.Size == size);

            if (cartItem != null)
            {
                if (product.Stock < cartItem.Quantity + quantity)
                    return false;

                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new Cart
                {
                    UserId = userId,
                    ProductId = productId,
                    Size = size,
                    Quantity = quantity
                };

                _context.Carts.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CartResponse>> GetCart(int userId)
        {
            return await _context.Carts
                .Where(c => c.UserId == userId)
                .Select(c => new CartResponse
                {
                    CartId = c.CartId,
                    ProductId = c.ProductId,
                    Name = c.Product.Name,
                    Price = c.Product.Price,
                    ImageUrl = c.Product.Images
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault(),
                    Size = c.Size,
                    Quantity = c.Quantity,
                    SubTotal = c.Product.Price * c.Quantity,
                    Stock = c.Product.Stock
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateQuantity(
            int userId, int productId, string size, int quantity)
        {
            if (quantity <= 0 || string.IsNullOrWhiteSpace(size))
                return false;

            var cartItem = await _context.Carts
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c =>
                    c.UserId == userId &&
                    c.ProductId == productId &&
                    c.Size == size);

            if (cartItem == null)
                return false;

            if (quantity > cartItem.Product.Stock)
                return false;

            cartItem.Quantity = quantity;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromCart(
            int userId, int productId, string size)
        {
            var cartItem = await _context.Carts
                .FirstOrDefaultAsync(c =>
                    c.UserId == userId &&
                    c.ProductId == productId &&
                    c.Size == size);

            if (cartItem == null)
                return false;

            _context.Carts.Remove(cartItem);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
