namespace MenShop_Assignment.Datas
{
    public class ImagesProduct : Image
    {
        public int ProductDetailId { get; set; }
        public ProductDetail? ProductDetail { get; set; }
    }
}
