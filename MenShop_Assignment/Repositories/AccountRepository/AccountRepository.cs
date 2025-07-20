using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using MenShop_Assignment.Models.Account;
using MenShop_Assignment.Services;
using MenShop_Assignment.Services.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MenShop_Assignment.Repositories.AccountRepository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;

        public AccountRepository(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<ApiResponseModel<object>> RegisterAsync(AccountRegister model)
        {
            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            if (emailExists != null)
            {
                if (await _userManager.IsLockedOutAsync(emailExists))
                {
                    return new ApiResponseModel<object>(
                        isSuccess: false,
                        message: "Tài khoản này đang bị khóa.",
                        data: null,
                        statusCode: 400
                    );
                }

                return new ApiResponseModel<object>(
                    isSuccess: false,
                    message: "Email đã được sử dụng. Vui lòng sử dụng email khác!",
                    data: null,
                    statusCode: 400
                );
            }

            var phoneExists = await _userManager.Users
                .AnyAsync(u => u.PhoneNumber == model.PhoneNumber);
            if (phoneExists)
            {
                return new ApiResponseModel<object>(
                    isSuccess: false,
                    message: "Số điện thoại đã được sử dụng.",
                    data: null,
                    statusCode: 400
                );
            }

            var user = new User
            {
                FullName = model.FullName,
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                BirthDate = model.BirthDate,
                CreatedDate = DateTime.Now,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return new ApiResponseModel<object>(
                    isSuccess: false,
                    message: "Đăng ký tài khoản thất bại.",
                    data: null,
                    statusCode: 400,
                    errors: result.Errors.Select(e => e.Description).ToList()
                );
            }

            await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);

            var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
            if (!roleResult.Succeeded)
            {
                return new ApiResponseModel<object>(
                    isSuccess: false,
                    message: "Thêm vai trò thất bại.",
                    data: null,
                    statusCode: 400,
                    errors: roleResult.Errors.Select(e => e.Description).ToList()
                );
            }

            return new ApiResponseModel<object>(
                isSuccess: true,
                message: "Đăng ký tài khoản thành công.",
                data: null,
                statusCode: 200
            );
        }


        public async Task<ApiResponseModel<LoginResponseDTO>> LoginAsync(AccountLogin model)
        {
            User? user = null;

            if (new EmailAddressAttribute().IsValid(model.Identifier))
            {
                user = await _userManager.FindByEmailAsync(model.Identifier);
            }
            else
            {
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.Identifier);
            }

            if (user == null)
            {
                return new ApiResponseModel<LoginResponseDTO>(
                    isSuccess: false,
                    message: "Tên đăng nhập hoặc mật khẩu không chính xác.",
                    data: null,
                    statusCode: 401
                );
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                return new ApiResponseModel<LoginResponseDTO>(
                    isSuccess: false,
                    message: "Tài khoản của bạn đã bị khóa.",
                    data: null,
                    statusCode: 401
                );
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                await _userManager.AccessFailedAsync(user);
                return new ApiResponseModel<LoginResponseDTO>(
                    isSuccess: false,
                    message: "Tên đăng nhập hoặc mật khẩu không chính xác.",
                    data: null,
                    statusCode: 401
                );
            }

            await _userManager.ResetAccessFailedCountAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            return new ApiResponseModel<LoginResponseDTO>(
                isSuccess: true,
                message: "Đăng nhập thành công.",
                data: new LoginResponseDTO
                {
                    UserName = !string.IsNullOrEmpty(user.FullName) ? user.FullName : user.UserName,
                    Email = user.Email,
                    Token = await _tokenService.CreateToken(user, roles)
                },
                statusCode: 200
            );
        }




    }
}

