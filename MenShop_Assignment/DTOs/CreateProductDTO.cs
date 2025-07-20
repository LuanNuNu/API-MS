using MenShop_Assignment.Datas;

namespace MenShop_Assignment.DTOs
{
    public class CreateProductDTO
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public bool Status { get; set; }
        public List<ProductDetailDTO>? ProductDetails { get; set; }
    }
}
