namespace MenShop_Assignment.Datas
{
    public class Storage
    {
        public int StorageId { get; set; }
        public int? CategoryId { get; set; }
        public CategoryProduct? CategoryProduct { get; set; }
        public string? ManagerId { get; set; }
        public User? Manager { get; set; }
        public ICollection<StorageDetail>? StorageDetails { get; set; }
        public ICollection<InputReceipt>? InputReceipts { get; set; }
        public ICollection<OutputReceipt>? OutputReceipts { get;set; }
    }
}
