using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace KidsWearAPI.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public string Size { get; set; } = "";

        public string ProductName { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Precision(10, 2)]
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        [Precision(10, 2)]
        public decimal SubTotal { get; set; }

        public Order Order { get; set; } = null!;
    }
}
