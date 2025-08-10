using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Repositories.ForgotPasswordRepository
{
    public interface IForgotPasswordRepository
    {
        Task<ApiResponseModel<object>> SendOtpAsync(string email);
        Task<ApiResponseModel<object>> VerifyOtpAsync(string email, string otp);
        Task<ApiResponseModel<object>> ResetPasswordAsync(string email, string newPassword);
    }
}
