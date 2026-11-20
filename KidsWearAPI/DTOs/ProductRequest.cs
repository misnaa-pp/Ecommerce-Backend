

using System.ComponentModel.DataAnnotations;

namespace KidsWearAPI.DTOs
{
    public class ProductRequest
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; } = "";

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(0.01, 1000000,
            ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Range(0, 1000000)]
        public decimal OriginalPrice { get; set; }

        [Range(0, 100)]
        public decimal Discount { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public List<string> Sizes { get; set; } = new();

        public List<string> Colors { get; set; } = new();

        public List<string> Images { get; set; } = new();
    }
}
