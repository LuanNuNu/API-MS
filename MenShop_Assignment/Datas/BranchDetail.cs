namespace MenShop_Assignment.Datas
{
    public class BranchDetail
    {
        public int BranchId { get; set; }
        public Branch? Branch { get; set; }
        public int ProductDetailId { get; set; }
        public ProductDetail? ProductDetail { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
    }
}
