
namespace KidsWearAPI.DTOs
{
    public class AdminOrderResponse
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = "";
        public string CustomerEmail { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = "";
    }
}
