namespace MenShop_Assignment.DTOs
{
    public class CreateOrderDetailDTO
    {
        public int ProductDetailId { get; set; }
        public int Quantity { get; set; }
        public decimal? SellPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
    }
}
