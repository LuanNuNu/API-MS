namespace MenShop_Assignment.Models.VNPay
{
    public class CartDetailViewModel
    {
        public int DetailId { get; set; }


        public string? ProductName { get; set; }
        public string? SizeName { get; set; }
        public string? ColorName { get; set; }
        public string? FabricName { get; set; }
        public List<string>? Images { get; set; }


        public decimal? SellPrice { get; set; }     // Giá gốc
        public decimal? DiscountedPrice { get; set; }    // Giá đã giảm
        public decimal? DiscountPercent { get; set; } //% giảm 

        public int? Quantity { get; set; }
    }

}
