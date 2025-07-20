using MenShop_Assignment.Datas;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Mapper
{
	public static class PaymentMapper
	{
		public static PaymentViewModel ToPaymentViewModel(Payment payment)
		{
			return new PaymentViewModel
			{
				PaymentId = payment.PaymentId,
				OrderId = payment.OrderId,
				Amount = payment.Amount,
				PaymentDate = payment.PaymentDate,
                Method = payment.Method,
                Status = payment.Status,
                TransactionCode = payment.TransactionCode,
				PaymentProvider = payment.PaymentProvider,
				Notes = payment.Notes,
				StaffId = payment.StaffId,
				Discounts = payment.Discounts?.Select(ToPaymentDiscountViewModel).ToList() ?? [],
			};
		}
		public static PaymentDiscountViewModel ToPaymentDiscountViewModel(PaymentDiscount payment)
		{
			return new PaymentDiscountViewModel
			{
				DiscountId = payment.DiscountId,
				PaymentId = payment.PaymentId,
				CouponCode = payment.CouponCode,
				DiscountAmount = payment.DiscountAmount,
				DiscountPercentage = payment.DiscountPercentage,
				Type = payment.Type,
			};
		}
	}
}
