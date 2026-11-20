namespace KidsWearAPI.DTOs.Payment
{
    public class VerifyRazorpayPaymentRequest
    {
        public int OrderId { get; set; }
        public string RazorpayOrderId { get; set; } = "";
        public string RazorpayPaymentId { get; set; } = "";
        public string RazorpaySignature { get; set; } = "";
    }
}