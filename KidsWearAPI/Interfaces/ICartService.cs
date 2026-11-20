using KidsWearAPI.DTOs;

namespace KidsWearAPI.Interfaces
{
    public interface ICartService
    {
        Task<bool> AddToCart(int userId, int productId, string size, int quantity);

        Task<IEnumerable<CartResponse>> GetCart(int userId);

        Task<bool> UpdateQuantity(int userId, int productId, string size, int quantity);

        Task<bool> RemoveFromCart(int userId, int productId, string size);
    }
}