using Microsoft.AspNetCore.Mvc;
using MenShop_Assignment.Repositories.ForgotPasswordRepository;
using MenShop_Assignment.DTOs.ForgotPassword;
using MenShop_Assignment.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace MenShop_Assignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly IForgotPasswordRepository _forgotRepo;

        public ForgotPasswordController(IForgotPasswordRepository forgotRepo)
        {
            _forgotRepo = forgotRepo;
        }

        [HttpPost("send-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> SendOtp([FromBody] string email)
        {
            var result = await _forgotRepo.SendOtpAsync(email);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("verify-otp")]
        [AllowAnonymous]

        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest model)
        {
            var result = await _forgotRepo.VerifyOtpAsync(model.SessionId, model.Otp);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]

        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
            var result = await _forgotRepo.ResetPasswordAsync(model.SessionId, model.NewPassword);
            return StatusCode(result.StatusCode, result);
        }
    }
}
