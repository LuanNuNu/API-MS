using MenShop_Assignment.Datas;

namespace MenShop_Assignment.Models
{
	public class HistoryPriceViewModel
	{
		public int Id { get; set; }
		public string? ProductName { get; set; }
		public string? ProductColor { get; set; }
		public string? ProductFabric { get; set; }
		public string? ProductSize { get; set; }
		public decimal? InputPrice { get; set; }
		public decimal? SellPrice { get; set; }
		public DateTime? UpdatedDate { get; set; }
	}
}
