namespace MenShop_Assignment.Datas
{
    public class InputReceiptDetail
    {
        public int ReceiptId {  get; set; }
        public InputReceipt? InputReceipt { get; set; }
        public int ProductDetailId { get; set; }
        public ProductDetail? ProductDetail { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
    }
}
