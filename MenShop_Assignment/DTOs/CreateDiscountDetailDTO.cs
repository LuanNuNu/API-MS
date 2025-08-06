namespace MenShop_Assignment.DTOs
{
    public class CreateDiscountDetailDTO
    {
        public int discountPriceId { get; set; }
        public List<int> productDetailIds { get; set; }
    }
}
