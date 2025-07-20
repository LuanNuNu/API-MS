using MenShop_Assignment.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace MenShop_Assignment.Models.AccountModels
{
    public class EmployeeUpdateDTO : UserBaseUpdateDTO
    {
        public int? BranchId { get; set; }

        [StringLength(200)]
        public string? EmployeeAddress { get; set; }
        public string? NewPassword { get; set; }
    }
}
