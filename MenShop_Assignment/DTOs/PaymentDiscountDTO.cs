using MenShop_Assignment.Extensions;

namespace MenShop_Assignment.DTOs
{
    public class PaymentDiscountDTO
    {
        public string? CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DiscountType Type { get; set; }
    }

}
