namespace MenShop_Assignment.Datas
{
    public class CartDetail
    {
        public int CartId { get; set; }
        public Cart? Cart { get; set; }
        public int? ProductDetailId { get; set; }
        public ProductDetail? ProductDetail { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
    }
}
