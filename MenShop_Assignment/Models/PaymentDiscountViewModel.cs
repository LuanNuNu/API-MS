using MenShop_Assignment.Datas;
using MenShop_Assignment.Extensions;

namespace MenShop_Assignment.Models
{
    public class PaymentDiscountViewModel
    {
		public int DiscountId { get; set; }
		public string? PaymentId { get; set; }
		public string? CouponCode { get; set; }
		public decimal DiscountAmount { get; set; }
		public decimal DiscountPercentage { get; set; }
		public DiscountType? Type { get; set; }
	}
}
