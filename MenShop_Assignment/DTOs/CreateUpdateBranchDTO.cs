using MenShop_Assignment.Datas;

namespace MenShop_Assignment.DTOs
{
	public class CreateUpdateBranchDTO
	{
		public int? Id { get; set; }
		public AddressInfo? Address { get; set; }

        public bool IsOnline { get; set; }
		public string? Name { get; set; }
    }
}
