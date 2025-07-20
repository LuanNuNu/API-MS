using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using MenShop_Assignment.Models.Account;
using Microsoft.AspNetCore.Identity;

namespace MenShop_Assignment.Repositories.AccountRepository
{
    public interface IAccountRepository
    {
        Task<ApiResponseModel<object>> RegisterAsync(AccountRegister model);
        Task<ApiResponseModel<LoginResponseDTO>> LoginAsync(AccountLogin model);
    }
}