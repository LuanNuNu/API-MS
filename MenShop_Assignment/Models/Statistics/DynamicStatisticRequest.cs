namespace MenShop_Assignment.Models.Statistics
{
	public class DynamicStatisticRequest
	{
		public StatisticMode Mode { get; set; }
		public int? Year { get; set; }         // Bắt buộc với Month/Day
		public int? Month { get; set; }        // Bắt buộc với Day
		public int? BranchId { get; set; }     // null => tất cả chi nhánh
	}
}
