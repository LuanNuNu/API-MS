namespace MenShop_Assignment.Datas
{
    public class ProductDetail
    {
        public int DetailId { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int SizeId { get; set; }
        public Size? Size { get; set; }
        public int ColorId { get; set; }
        public Color? Color { get; set; }
        public int FabricId { get; set; }
        public Fabric? Fabric { get; set; }
        public ICollection<HistoryPrice>? HistoryPrices { get; set; }
        public ICollection<ImagesProduct>? Images {  get; set; }
        public ICollection<StorageDetail>? StorageDetails { get; set; }
        public ICollection<InputReceiptDetail>? InputReceiptDetails { get; set; }
        public ICollection<BranchDetail>? BranchDetails { get; set; }
        public ICollection<CartDetail>? CartDetails { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
        public ICollection<OutputReceiptDetail>? OutputReceiptDetails { get; set; }
    }
}
