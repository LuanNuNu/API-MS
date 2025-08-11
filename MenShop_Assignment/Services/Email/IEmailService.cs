namespace MenShop_Assignment.Services.Email
{
    public interface IEmailService
    {
        Task SendOtpAsync(string toEmail, string otp);
    }
}
