namespace MenShop_Assignment.Models
{
    public class DiscountPriceViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
