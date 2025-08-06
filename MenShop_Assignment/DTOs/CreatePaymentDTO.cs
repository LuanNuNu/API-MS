using MenShop_Assignment.Extensions;

namespace MenShop_Assignment.DTOs
{
    public class CreatePaymentDTO
    {
        public decimal Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public string? TransactionCode { get; set; }
        public string? PaymentProvider { get; set; }
        public string? Notes { get; set; }
        public ICollection<CreatePaymentDiscountDTO>? Discounts { get; set; }
    }
}
