using System.ComponentModel.DataAnnotations;

namespace MenShop_Assignment.Models.Account
{
    public class AccountLogin
    {
        [Required(ErrorMessage = "Vui lòng nhập email hoặc số điện thoại.")]
        public string Identifier { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        public string Password { get; set; } = null!;
    }
}
