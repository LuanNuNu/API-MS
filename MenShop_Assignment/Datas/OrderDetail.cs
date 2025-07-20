namespace MenShop_Assignment.Datas
{
    public class OrderDetail
    {
        public string OrderId { get; set; }
        public Order? Order { get; set; }
        public int ProductDetailId { get; set; }
        public ProductDetail? ProductDetail { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
    }
}
