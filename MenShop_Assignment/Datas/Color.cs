namespace MenShop_Assignment.Datas
{
    public class Color
    {
        public int ColorId { get; set; }
        public string? Name { get; set; }
        public ICollection<ProductDetail>? ProductDetails { get; set; }
    }
}
