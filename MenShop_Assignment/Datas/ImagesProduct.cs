namespace MenShop_Assignment.Datas
{
    public class ImagesProduct
    {
        public int Id { get; set; }
        public string? Path { get; set; }
        public string? FullPath { get; set; }
        public int ProductDetailId { get; set; }
        public ProductDetail? ProductDetail { get; set; }
    }
}
