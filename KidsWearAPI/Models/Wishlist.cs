using System.ComponentModel.DataAnnotations;

namespace KidsWearAPI.Models
{
    public class Wishlist
    {
        [Key]
        public int WishlistId { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;

        public Product Product { get; set; } = null!;
    }
}