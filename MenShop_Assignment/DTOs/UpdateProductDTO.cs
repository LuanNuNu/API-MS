namespace MenShop_Assignment.DTOs
{
    public class UpdateProductDTO
    {
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
        public int CategoryId { get; set; }
        public List<UpdateProductDetailDTO> ProductDetails { get; set; } = new();
    }
}
