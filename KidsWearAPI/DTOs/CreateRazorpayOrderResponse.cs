namespace KidsWearAPI.DTOs.Payment
{
    public class CreateRazorpayOrderResponse
    {
        public string RazorpayOrderId { get; set; } = "";
        public string KeyId { get; set; } = "";
        public int Amount { get; set; }
        public string Currency { get; set; } = "INR";
    }
}