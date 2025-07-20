namespace MenShop_Assignment.Datas
{
    public class Size
    {
        public int SizeId { get; set; }
        public string? Name { get; set; }
        public ICollection<ProductDetail>? ProductDetails { get; set; }
    }
}
