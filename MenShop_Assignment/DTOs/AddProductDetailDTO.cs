namespace MenShop_Assignment.DTOs
{
    public class AddProductDetailDTO
    {
        public int ProductId { get; set; }
        public List<ProductDetailDTO> Details { get; set; }
    }
}
