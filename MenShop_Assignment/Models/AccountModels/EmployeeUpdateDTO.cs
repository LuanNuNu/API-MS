using MenShop_Assignment.Datas;
using MenShop_Assignment.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace MenShop_Assignment.Models.AccountModels
{
    public class EmployeeUpdateDTO : UserBaseUpdateDTO
    {
        public int? BranchId { get; set; }

        [StringLength(200)]
        public string? NewPassword { get; set; }
        public AddressInfo? WorkArea { get; set; }
    }
}
