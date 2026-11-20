
using System.ComponentModel.DataAnnotations;

namespace KidsWearAPI.DTOs
{
    public class UpdateOrderStatusRequest
    {
        [Required]
        [RegularExpression(
            "^(Pending|Processing|Shipped|Delivered|Cancelled)$",
            ErrorMessage = "Invalid order status."
        )]
        public string Status { get; set; } = "";
    }
}
