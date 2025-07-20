using MenShop_Assignment.Datas;

namespace MenShop_Assignment.Models
{
	public class BranchViewModel
	{
		public int BranchId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? ManagerName { get; set; }
        public bool IsOnline { get; set; }
        public ICollection<ProductDetailViewModel>? BranchDetails { get; set; }
	}
}
