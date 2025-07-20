namespace MenShop_Assignment.DTOs
{
    public class UpdateProductDetailDTO
    {
        public int DetailId { get; set; }
        public int SizeId { get; set; }
        public int ColorId { get; set; }
        public int FabricId { get; set; }
        public List<UpdateImageDTO>? Images { get; set; }
    }
}
