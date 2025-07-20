using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using MenShop_Assignment.Models.Account;
using MenShop_Assignment.Repositories.AccountRepository;
using MenShop_Assignment.Services;
using MenShop_Assignment.Services.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepo;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signinManager;
        public AccountController(UserManager<User> userManager, IAccountRepository accountRepo, IConfiguration configuration, SignInManager<User> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _accountRepo = accountRepo;
            _configuration = configuration;
            _signinManager = signInManager;
            _tokenService = tokenService;

        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] AccountRegister model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseModel<object>(
                    false,
                    "Dữ liệu đầu vào không hợp lệ.",
                    null,
                    400,
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                ));
            }

            var response = await _accountRepo.RegisterAsync(model);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, response);

            return Ok(response);
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AccountLogin model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseModel<object>(
                    false,
                    "Dữ liệu đầu vào không hợp lệ.",
                    null,
                    400,
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                ));
            }

            var response = await _accountRepo.LoginAsync(model);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, response);

            return Ok(response);
        }



    }
}
