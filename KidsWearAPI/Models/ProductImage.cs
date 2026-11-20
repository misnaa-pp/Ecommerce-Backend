using System.ComponentModel.DataAnnotations;

namespace KidsWearAPI.Models
{
    public class ProductImage
    {
        [Key]
        public int ProductImageId { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
