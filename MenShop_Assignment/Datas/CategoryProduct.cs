namespace MenShop_Assignment.Datas
{
    public class CategoryProduct
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public ICollection<Product>? Products { get; set; }
        public Storage? Storage { get; set; }
    }
}
