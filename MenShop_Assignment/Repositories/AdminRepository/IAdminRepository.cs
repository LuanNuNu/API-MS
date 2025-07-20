using MenShop_Assignment.Models;
using MenShop_Assignment.Models.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MenShop_Assignment.Models.AccountModels;

namespace MenShop_Assignment.Repositories.AdminRepository
{
    public interface IAdminRepository
    {
        Task<ApiResponseModel<object>> CreateUserByAdminAsync(AccountRegister model);
        Task<ApiResponseModel<IEnumerable<UserViewModel>>> GetUsersAsync(string? userId, string? email, string? roleId, int? branchId);
        Task<ApiResponseModel<object>> UpdateEmployeeByIdAsync(string id, EmployeeUpdateDTO model);
        Task<ApiResponseModel<object>> UpdateCustomerByIdAsync(string id, CustomerUpdateDTO model, bool isSelfUpdate = false);
        Task<ApiResponseModel<object>> AddClaimAsync(string userId, string claimType, string claimValue);
        Task<ApiResponseModel<object>> ToggleUserStatusAsync(string userId);
        Task<List<IdentityRole>> GetAllRolesAsync();
    }
}
