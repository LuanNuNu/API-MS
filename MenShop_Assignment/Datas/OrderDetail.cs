namespace MenShop_Assignment.Datas
{
    public class OrderDetail
    {
        public string OrderId { get; set; }
        public Order? Order { get; set; }

        public int ProductDetailId { get; set; }
        public ProductDetail? ProductDetail { get; set; }

        public int? Quantity { get; set; }

        // Giá gốc tại thời điểm mua
        public decimal? SellPrice { get; set; }

        // Giá đã giảm tại thời điểm mua (nếu không giảm thì = OriginalPrice)
        public decimal? DiscountedPrice { get; set; }
    }

}
