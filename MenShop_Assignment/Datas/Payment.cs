using MenShop_Assignment.Extensions;
namespace MenShop_Assignment.Datas
{
	public class Payment
	{
		public string PaymentId { get; set; }
		public string OrderId { get; set; }
		public Order? Order { get; set; }
		public decimal Amount { get; set; }
		public DateTime? PaymentDate { get; set; }
		public PaymentMethod Method { get; set; }
		public PaymentStatus Status { get; set; }
		public string? TransactionCode { get; set; }
		public string? PaymentProvider { get; set; }
		public string? Notes { get; set; }
		public string? StaffId { get; set; }
		public User? Staff { get; set; }
		public ICollection<PaymentDiscount>? Discounts { get; set; }
	}
}
