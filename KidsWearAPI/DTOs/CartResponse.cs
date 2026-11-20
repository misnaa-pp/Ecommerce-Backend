namespace KidsWearAPI.DTOs
{
    public class CartResponse
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Size { get; set; } = "";
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public int Stock { get; set; }
    }
}