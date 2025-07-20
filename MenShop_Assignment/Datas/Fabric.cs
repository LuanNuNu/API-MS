namespace MenShop_Assignment.Datas
{
    public class Fabric
    {
        public int FabricId { get; set; }
        public string? Name { get; set; }
        public ICollection<ProductDetail>? ProductDetails { get; set; }
    }
}
