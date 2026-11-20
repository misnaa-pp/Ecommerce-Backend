using KidsWearAPI.DTOs;

namespace KidsWearAPI.Interfaces
{
    public interface IWishlistService
    {
        Task<bool> AddToWishlist(int userId, int productId);

        Task<IEnumerable<WishlistResponse>> GetWishlist(int userId);

        Task<bool> RemoveFromWishlist(int userId, int productId);
    }
}