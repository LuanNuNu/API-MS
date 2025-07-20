using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using MenShop_Assignment.Models.Account;
using MenShop_Assignment.Models.AccountModels;
using MenShop_Assignment.Repositories.AdminRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MenShop_Assignment.APIControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAdminRepository _adminRepo;

        public AdminController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            IAdminRepository adminRepo)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _adminRepo = adminRepo;
        }

        [HttpPost("create-staff")]
        public async Task<IActionResult> CreateUserByAdmin(AccountRegister model)
        {
            var result = await _adminRepo.CreateUserByAdminAsync(model);

            if (!result.IsSuccess)
            {
                if (result.Errors != null && result.Errors.Any())
                    return StatusCode(result.StatusCode, result);
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }


        [HttpGet("users")]
        //[Authorize(Roles = "Admin")]
        [Authorize]

        public async Task<IActionResult> GetUsers([FromQuery] string? userId, [FromQuery] string? email, [FromQuery] string? roleId, [FromQuery] int? branchId)
        {
            var result = await _adminRepo.GetUsersAsync(userId, email, roleId, branchId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("add-claim")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddClaim(string userId, string claimType, string claimValue)
        {
            var result = await _adminRepo.AddClaimAsync(userId, claimType, claimValue);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPut("update-employee/{id}")]
        [Authorize(Roles = "Admin,BranchManager")]
        public async Task<IActionResult> UpdateEmployee(string id, [FromBody] EmployeeUpdateDTO model)
        {
            var result = await _adminRepo.UpdateEmployeeByIdAsync(id, model);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("update-customer/{id}")]
        [Authorize(Roles = "BranchEmployee ,BranchManager, Admin")]
        public async Task<IActionResult> UpdateCustomer(string id, [FromBody] CustomerUpdateDTO model)
        {
            var result = await _adminRepo.UpdateCustomerByIdAsync(id, model, isSelfUpdate: false);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPut("update-profile")]
        [Authorize(Roles = "Customer, Admin")]
        public async Task<IActionResult> UpdateOwnProfile([FromBody] CustomerUpdateDTO model)
        {
            var allClaims = User.Claims.Select(c => $"{c.Type} = {c.Value}").ToList();
            foreach (var claim in allClaims)
            {
                Console.WriteLine($"CLAIM: {claim}");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"USER ID: {userId}");

            if (userId == null)
                return Unauthorized("Không xác định được người dùng.");

            var result = await _adminRepo.UpdateCustomerByIdAsync(userId, model, isSelfUpdate: true);    
            return StatusCode(result.StatusCode, result);
        }



        [HttpPut("toggle-user-status/{id}")]
        [Authorize(Roles = "Admin,BranchManager")]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            var response = await _adminRepo.ToggleUserStatusAsync(id);
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("roles")]
        [Authorize(Roles = "Admin,BranchManager")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _adminRepo.GetAllRolesAsync();

            var roleDtos = roles
            .OrderBy(r => r.Name)
                .Select(r => new RoleDTO
                {
                    Id = r.Id,
                    Name = r.Name
                })
            .ToList();

            return Ok(new ApiResponseModel<List<RoleDTO>>(
                isSuccess: true,
                message: "Lấy danh sách vai trò thành công.",
                data: roleDtos,
                statusCode: 200
            ));
        }

        [HttpGet("users/me")]
        public async Task<IActionResult> GetCurrentUserAsync()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponseModel<UserViewModel>(
                    isSuccess: false,
                    message: "Không xác định được người dùng.",
                    data: null,
                    statusCode: 401
                ));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new ApiResponseModel<UserViewModel>(
                    isSuccess: false,
                    message: $"Không tìm thấy người dùng với Id: {userId}",
                    data: null,
                    statusCode: 404
                ));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            var vm = UserMapper.MapToViewModel(user, roles, claims);

            return Ok(new ApiResponseModel<UserViewModel>(
                isSuccess: true,
                message: "Lấy thông tin người dùng thành công.",
                data: vm,
                statusCode: 200
            ));
        }

    }
}
