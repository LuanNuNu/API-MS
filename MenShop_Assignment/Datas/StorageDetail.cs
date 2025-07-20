namespace MenShop_Assignment.Datas
{
    public class StorageDetail
    {
        public int ProductDetailId {  get; set; }
        public ProductDetail? ProductDetail { get; set; }
        public int StorageId {  get; set; }
        public Storage? Storage { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
    }
}
