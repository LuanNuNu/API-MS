using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;

namespace MenShop_Assignment.Models
{
	public class ProductDetailViewModel : ProductDetailBaseModel
    {
		public decimal? InputPrice { get; set; }
        //public List<ImageProductViewModel> Images { get; set; }

        public decimal? DiscountPercent { get; set; }
        public DateTime? LatestPriceDate { get; set; }

    }
}
