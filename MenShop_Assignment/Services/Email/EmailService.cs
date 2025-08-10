using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService
{
    private readonly string _fromEmail = "vandang3082006@gmail.com";
    private readonly string _appPassword = "kinuxsxjlqssheus";

    public async Task SendOtpAsync(string toEmail, string otp)
    {
        using var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_fromEmail, _appPassword)
        };

        var mail = new MailMessage
        {
            From = new MailAddress(_fromEmail, "MenShop"),
            Subject = "Mã OTP khôi phục mật khẩu",
            Body = $"Mã OTP của bạn là {otp}. Mã hết hạn trong 5 phút.",
            IsBodyHtml = false
        };

        mail.To.Add(toEmail);
        await client.SendMailAsync(mail);
    }
}
