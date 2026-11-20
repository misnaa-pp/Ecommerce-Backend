
namespace KidsWearAPI.DTOs
{
    public class AdminOrderDetailsResponse
    {
        
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; } = "";
        public string PaymentStatus { get; set; } = "";
        public string PaymentMethod { get; set; } = "";

        public int UserId { get; set; }
        public string CustomerName { get; set; } = "";
        public string CustomerEmail { get; set; } = "";

        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string AddressLine { get; set; } = "";
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public string Country { get; set; } = "";

        public decimal SubTotal { get; set; }
        public decimal ShippingCharge { get; set; }
        public decimal TotalAmount { get; set; }

        public List<AdminOrderItemResponse> Items { get; set; } = new();
    }

    public class AdminOrderItemResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string Size { get; set; } = "";
        public string? ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }
}
