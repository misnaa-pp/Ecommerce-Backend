using System.ComponentModel.DataAnnotations;

namespace KidsWearAPI.Models
{
    public class ProductColor
    {
        [Key]
        public int ProductColorId { get; set; }

        [Required]
        public string Color { get; set; } = string.Empty;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
