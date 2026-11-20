
namespace KidsWearAPI.DTOs
{
    public class WishlistResponse
    {
        public int WishlistId { get; set; }

        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal OriginalPrice { get; set; }

        public int Stock { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime AddedAt { get; set; }
    }
}