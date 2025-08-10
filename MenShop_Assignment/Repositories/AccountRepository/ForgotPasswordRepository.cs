using System.Collections.Concurrent;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using Microsoft.AspNetCore.Identity;
using MenShop_Assignment.Services;
using MenShop_Assignment.Datas;

namespace MenShop_Assignment.Repositories.ForgotPasswordRepository
{
    public class ForgotPasswordRepository : IForgotPasswordRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly EmailService _emailService;
        private static ConcurrentDictionary<string, (string Email, string Otp, DateTime Expire, bool IsVerified)> _otpStore = new();

        public ForgotPasswordRepository(UserManager<User> userManager, EmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }
        public async Task<ApiResponseModel<object>> SendOtpAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ApiResponseModel<object>(false, "Email không tồn tại", null, 400);
            var otp = new Random().Next(100000, 999999).ToString();
            var sessionId = Guid.NewGuid().ToString();
            _otpStore[sessionId] = (email, otp, DateTime.UtcNow.AddMinutes(5), false);
            await _emailService.SendOtpAsync(email, otp);

            return new ApiResponseModel<object>(
                true,
                "OTP đã gửi về Gmail",
                new { sessionId },
                200
            );
        }
        public Task<ApiResponseModel<object>> VerifyOtpAsync(string sessionId, string otp)
        {
            if (_otpStore.TryGetValue(sessionId, out var data))
            {
                if (data.Expire > DateTime.UtcNow && data.Otp == otp)
                {
                    _otpStore[sessionId] = (data.Email, data.Otp, data.Expire, true);

                    return Task.FromResult(new ApiResponseModel<object>(
                        true,
                        "OTP hợp lệ",
                        null,
                        200
                    ));
                }
            }

            return Task.FromResult(new ApiResponseModel<object>(
                false,
                "OTP sai hoặc đã hết hạn",
                null,
                400
            ));
        }
        public async Task<ApiResponseModel<object>> ResetPasswordAsync(string sessionId, string newPassword)
        {
            if (!_otpStore.TryGetValue(sessionId, out var data))
                return new ApiResponseModel<object>(false, "Phiên làm việc không hợp lệ hoặc đã hết hạn", null, 400);

            if (!data.IsVerified)
                return new ApiResponseModel<object>(false, "OTP chưa được xác minh", null, 403);

            var user = await _userManager.FindByEmailAsync(data.Email);
            if (user == null)
                return new ApiResponseModel<object>(false, "Email không tồn tại", null, 400);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                _otpStore.TryRemove(sessionId, out _);

                return new ApiResponseModel<object>(
                    true,
                    "Đổi mật khẩu thành công",
                    null,
                    200
                );
            }

            return new ApiResponseModel<object>(
                false,
                "Đổi mật khẩu thất bại",
                null,
                400,
                result.Errors.Select(e => e.Description).ToList()
            );
        }
    }
}
