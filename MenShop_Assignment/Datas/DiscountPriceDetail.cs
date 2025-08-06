namespace MenShop_Assignment.Datas
{
    public class DiscountPriceDetail
    {
        public int Id { get; set; }
        public int discountPriceId { get; set; }
        public DiscountPrice? DiscountPrice { get; set; }
        public int productDetailId { get; set; }
        public ProductDetail? ProductDetail { get; set; }
    }
}
