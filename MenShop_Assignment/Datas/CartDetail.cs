namespace MenShop_Assignment.Datas
{
    public class CartDetail
    {
        public int CartId { get; set; }
        public Cart? Cart { get; set; }

        public int? ProductDetailId { get; set; }
        public ProductDetail? ProductDetail { get; set; }
        public int? Quantity { get; set; }

        //public decimal? SellPrice { get; set; } // Giá gốc tại thời điểm thêm vào giỏ
        //public decimal? DiscountedPrice { get; set; } // Giá sau giảm
        //public decimal? DiscountPercent { get; set; } //% giảm 
    }

}
