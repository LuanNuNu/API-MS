using MenShop_Assignment.Datas;

namespace MenShop_Assignment.Models
{
	public class ProductDetailViewModel
	{
		public int DetailId { get; set; }
		public string? ProductName { get; set; }
		public string? SizeName { get; set; }
		public string? ColorName { get; set; }
		public string? FabricName { get; set; }
		public decimal? InputPrice { get; set; }
		public decimal? SellPrice { get; set;}
		public int? Quantity { get; set; }
		public ICollection<string>? Images { get; set; }
		//public List<ImageProductViewModel> Images { get; set; }
		public DateTime? LatestPriceDate { get; set; }
    }
}
