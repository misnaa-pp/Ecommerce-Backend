
using System.ComponentModel.DataAnnotations;

namespace KidsWearAPI.DTOs
{
    public class CheckoutRequest
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a valid address")]
        public int AddressId { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [RegularExpression(
            "^(CashOnDelivery|OnlinePayment)$",
            ErrorMessage = "Payment method must be CashOnDelivery or OnlinePayment")]
        public string PaymentMethod { get; set; } = "CashOnDelivery";

        public int? ProductId { get; set; }

        public string? Size { get; set; }

        public int? Quantity { get; set; }
    }
}
