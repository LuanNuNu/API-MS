namespace MenShop_Assignment.Datas
{
    public class Product
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public bool? Status { get; set; }
        public CategoryProduct? Category { get; set; }
        public ICollection<ProductDetail>? ProductDetails { get; set; }
        public ICollection<CollectionDetail> CollectionDetails { get; set; }
    }
}
