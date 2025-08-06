using MenShop_Assignment.Datas;
using MenShop_Assignment.Models.VNPay;

namespace MenShop_Assignment.Models
{
	public class CartViewModel
	{
		public int CartId { get; set; }
		public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public DateTime? CreatedDate { get; set; }
		//public ICollection<ProductDetailViewModel>? Details { get; set; }
		public ICollection<CartDetailViewModel>? Details { get; set; }
        public decimal? TotalAmount
		{
			get
			{
				if (Details == null || Details.Count == 0)
					return 0;
				return Details.Sum(x => (x.SellPrice ?? 0) * (x.Quantity ?? 0));
			}
		}
	}
}
