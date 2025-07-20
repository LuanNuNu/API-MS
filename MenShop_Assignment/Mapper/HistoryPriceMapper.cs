using MenShop_Assignment.Datas;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Mapper
{
	public static class HistoryPriceMapper
	{
		public static HistoryPriceViewModel ToHistoryPriceViewModel(HistoryPrice historyPrice)
		{
			return new HistoryPriceViewModel
			{
				Id = historyPrice.Id,
				InputPrice = historyPrice.InputPrice,
				SellPrice = historyPrice.SellPrice,
				ProductName = historyPrice.ProductDetail.Product.ProductName,
				ProductColor = historyPrice.ProductDetail.Color.Name,
				ProductSize = historyPrice.ProductDetail.Size.Name,
				ProductFabric = historyPrice.ProductDetail.Fabric.Name,
				UpdatedDate = historyPrice.UpdatedDate
			};
		}
	}
}
