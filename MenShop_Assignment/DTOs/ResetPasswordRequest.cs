namespace MenShop_Assignment.DTOs.ForgotPassword
{
    public class ResetPasswordRequest
    {
        public string SessionId { get; set; }
        public string NewPassword { get; set; }
    }
}
