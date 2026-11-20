
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace KidsWearAPI.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Precision(10, 2)]
        public decimal SubTotal { get; set; }

        [Precision(10, 2)]
        public decimal ShippingCharge { get; set; }

        [Precision(10, 2)]
        public decimal TotalAmount { get; set; }

        public string OrderStatus { get; set; } = "Pending";

        public string PaymentStatus { get; set; } = "Pending";

        public string PaymentMethod { get; set; } = "CashOnDelivery";

        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = "India";
        public bool IsBuyNow { get; set; } = false;

        public User User { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();
    }
}
