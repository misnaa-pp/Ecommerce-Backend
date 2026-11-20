using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KidsWearAPI.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Category { get; set; } = "";
        public string Description { get; set; } = "";

        [Precision(10,2)]
        public decimal Price { get; set; }

        [Precision(10,2)]
        public decimal OriginalPrice { get; set; }

        [Precision(5,2)]
        public decimal Discount { get; set; }
        public int Stock {  get; set; }
        public ICollection<ProductSize> Sizes { get; set; } = new List<ProductSize>();

        public ICollection<ProductColor> Colors { get; set; } = new List<ProductColor>();

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    }
}
