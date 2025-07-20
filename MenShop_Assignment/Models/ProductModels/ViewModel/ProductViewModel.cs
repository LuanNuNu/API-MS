namespace MenShop_Assignment.Models.ProductModels.ViewModel
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? CategoryName { get; set; }
        public List<ProductDetailViewModel>? ProductDetails { get; set; }
    }
}
