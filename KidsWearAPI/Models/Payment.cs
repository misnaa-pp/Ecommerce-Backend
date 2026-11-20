
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidsWearAPI.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        public string PaymentMethod { get; set; } = "OnlinePayment";

        public decimal Amount { get; set; }

        public string PaymentStatus { get; set; } = "Pending";

        public string? TransactionId { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public string? RazorpayOrderId { get; set; }
    }
}
