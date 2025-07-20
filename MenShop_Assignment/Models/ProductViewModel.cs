using MenShop_Assignment.Datas;

namespace MenShop_Assignment.Models
{
	public class ProductViewModel
	{
		public int ProductId { get; set; }
		public string? ProductName { get; set; }
		public string? Description { get; set; }
		public int? CategoryProductID { get; set; }
		public string? CategoryName { get; set; }
        public decimal? Price { get; set; }//
        public string? Status { get; set; }
		public string? Thumbnail {  get; set; }
		public ICollection<ProductDetailViewModel>? ProductDetails { get; set; }
	}
}
