namespace MenShop_Assignment.Datas
{
    public class OutputReceiptDetail
    {
        public int ReceiptId { get; set; }
        public OutputReceipt? OutputReceipt { get; set; }
        public int ProductDetailId { get; set; }
        public ProductDetail? ProductDetail { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
    }
}
