using System.ComponentModel.DataAnnotations;

namespace KidsWearAPI.Models
{
    public class ProductSize
    {
        [Key]
        public int ProductSizeId { get; set; }

        [Required]
        public string Size { get; set; } = "";
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

    }
}
