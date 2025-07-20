using MenShop_Assignment.Datas;


namespace MenShop_Assignment.Models
{
    public class StorageViewModel
    {
        public int StorageId { get; set; }
        public string? CategoryProduct { get; set; }
        public string? ManagerName { get; set; }
        public ICollection<ProductDetailViewModel>? StorageDetails { get; set; }
    }
}
