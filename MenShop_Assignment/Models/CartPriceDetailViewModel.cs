namespace MenShop_Assignment.Models
{
    public class CartPriceDetailViewModel
    {
        public int DetailId { get; set; }
        public decimal? SellPrice { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public int? Quantity { get; set; }
    }
}
