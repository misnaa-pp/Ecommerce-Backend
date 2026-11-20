
namespace KidsWearAPI.DTOs
{
    public class ProductResponse
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal Discount { get; set; }

        public string? Description { get; set; }

        public int Stock { get; set; }

        public List<string> Sizes { get; set; } = new();

        public List<string> Colors { get; set; } = new();

        public List<string> Images { get; set; } = new();
    }
}
