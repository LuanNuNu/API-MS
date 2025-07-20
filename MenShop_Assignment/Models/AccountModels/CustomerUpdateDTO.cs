using MenShop_Assignment.Models.Account;

namespace MenShop_Assignment.Models.AccountModels
{
    public class CustomerUpdateDTO : UserBaseUpdateDTO
    {
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
