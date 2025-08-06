namespace MenShop_Assignment.Datas
{
    public class DiscountPrice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public ICollection<DiscountPriceDetail> discountPriceDetails { get; set; }
    }
}
