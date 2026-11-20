using System.ComponentModel.DataAnnotations;

namespace KidsWearAPI.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public string Size { get; set; } = "";

        public int Quantity { get; set; } = 1;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;

        public Product Product { get; set; } = null!;
    }
}