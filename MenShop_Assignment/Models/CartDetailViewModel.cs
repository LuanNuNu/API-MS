namespace MenShop_Assignment.Models.VNPay
{
    public class CartDetailViewModel
    {
        public int DetailId { get; set; }
        public string? ProductName { get; set; }
        public string? SizeName { get; set; }
        public string? ColorName { get; set; }
        public string? FabricName { get; set; }
        public decimal? SellPrice { get; set; }
        public int? Quantity { get; set; }
        public List<string>? Images { get; set; }
    }
}
