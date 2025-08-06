// Repositories/AdminRepository.cs
using MenShop_Assignment.Datas;
using MenShop_Assignment.Models.Account;
using MenShop_Assignment.Repositories.AdminRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MenShop_Assignment.Models;
using System.Security.Claims;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models.AccountModels;


namespace MenShop_Assignment.Repositories.AdminRepository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminRepository(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApiResponseModel<object>> CreateUserByAdminAsync(AccountRegister model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return new ApiResponseModel<object>(
                    isSuccess: false,
                    message: "Email đã tồn tại.",
                    data: null,
                    statusCode: 400
                );

            var role = await _roleManager.FindByNameAsync(model.Role);
            if (role == null)
                return new ApiResponseModel<object>(
                    isSuccess: false,
                    message: $"Vai trò với Id {model.Role} không tồn tại.",
                    data: null,
                    statusCode: 400
                );

            if (string.Equals(role.Name, "Khách hàng", StringComparison.OrdinalIgnoreCase))
                return new ApiResponseModel<object>(
                    isSuccess: false,
                    message: "Admin không được phép tạo tài khoản với vai trò Khách hàng.",
                    data: null,
                    statusCode: 400
                );

            var user = new User
            {
                UserName = model.Email.Split('@')[0],
                FullName = model.FullName,
                Email = model.Email,
                Gender = model.Gender,
                PhoneNumber = model.PhoneNumber,
                BirthDate = model.BirthDate,
                CreatedDate = DateTime.Now,
                BranchId = model.BranchId,
                WorkArea = model.WorkArea,
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                var allErrors = string.Join(" ", createResult.Errors.Select(e => e.Description));

                return new ApiResponseModel<object>(
                    isSuccess: false,
                    message: $"Tạo tài khoản thất bại. {allErrors}",
                    data: null,
                    statusCode: 400
                );
            }


            var roleResult = await _userManager.AddToRoleAsync(user, role.Name);
            if (!roleResult.Succeeded)
                return new ApiResponseModel<object>(
                    isSuccess: false,
                    message: "Phân quyền thất bại.",
                    data: null,
                    statusCode: 400,
                    errors: roleResult.Errors.Select(e => e.Description).ToList()
                );

            return new ApiResponseModel<object>(
                isSuccess: true,
                message: $"Tạo tài khoản thành công với vai trò {role.Name}.",
                data: null,
                statusCode: 200
            );
        }


        public async Task<ApiResponseModel<IEnumerable<UserViewModel>>> GetUsersAsync(string? userId, string? email, string? roleId, int? branchId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ApiResponseModel<IEnumerable<UserViewModel>>(
                        isSuccess: false,
                        message: $"Không tìm thấy người dùng với Id: {userId}",
                        data: null,
                        statusCode: 404
                    );
                }

                var roles = await _userManager.GetRolesAsync(user);
                var claims = await _userManager.GetClaimsAsync(user);

                var vm = UserMapper.MapToViewModel(user, roles, claims);

                return new ApiResponseModel<IEnumerable<UserViewModel>>(
                    isSuccess: true,
                    message: "Lấy thông tin người dùng thành công.",
                    data: new[] { vm },
                    statusCode: 200
                );
            }

            if (!string.IsNullOrEmpty(email))
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new ApiResponseModel<IEnumerable<UserViewModel>>(
                        isSuccess: false,
                        message: $"Không tìm thấy người dùng với email: {email}",
                        data: null,
                        statusCode: 404
                    );
                }

                var roles = await _userManager.GetRolesAsync(user);
                var claims = await _userManager.GetClaimsAsync(user);

                var vm = UserMapper.MapToViewModel(user, roles, claims);

                return new ApiResponseModel<IEnumerable<UserViewModel>>(
                    isSuccess: true,
                    message: "Lấy thông tin người dùng thành công.",
                    data: new[] { vm },
                    statusCode: 200
                );
            }

            IQueryable<User> query = _userManager.Users;
            string message = "Lấy toàn bộ danh sách người dùng thành công.";

            if (!string.IsNullOrEmpty(roleId))
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return new ApiResponseModel<IEnumerable<UserViewModel>>(
                        isSuccess: false,
                        message: $"Không tìm thấy vai trò với Id: {roleId}",
                        data: null,
                        statusCode: 404
                    );
                }

                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                query = usersInRole.AsQueryable();
                message = "Lấy danh sách người dùng theo vai trò thành công.";
            }

            if (branchId.HasValue)
            {
                query = query.Where(u => u.BranchId == branchId.Value);
                message = !string.IsNullOrEmpty(roleId)
                    ? "Lấy danh sách người dùng theo vai trò và chi nhánh thành công."
                    : "Lấy danh sách người dùng theo chi nhánh thành công.";
            }

            var users = query.ToList();
            var result = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var claims = await _userManager.GetClaimsAsync(user);

                result.Add(UserMapper.MapToViewModel(user, roles, claims));
            }

            return new ApiResponseModel<IEnumerable<UserViewModel>>(
                isSuccess: true,
                message: message,
                data: result,
                statusCode: 200
            );
        }





        public async Task<ApiResponseModel<object>> UpdateEmployeeByIdAsync(string id, EmployeeUpdateDTO model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return ApiResponse.NotFound($"Không tìm thấy người dùng với ID: {id}");

            UpdateBasicUserInfo(user, model);
            user.WorkArea= model.WorkArea;
            user.BranchId = model.BranchId;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return ApiResponse.BadRequest("Cập nhật nhân viên thất bại.", updateResult.Errors);

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                await _userManager.RemovePasswordAsync(user);
                var pwdResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (!pwdResult.Succeeded)
                    return ApiResponse.BadRequest("Đổi mật khẩu thất bại.", pwdResult.Errors);
            }

            return ApiResponse.Success("Cập nhật nhân viên thành công.");
        }

        public async Task<ApiResponseModel<object>> UpdateCustomerByIdAsync(string id, CustomerUpdateDTO model, bool isSelfUpdate = false)

        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return ApiResponse.NotFound($"Không tìm thấy người dùng với ID: {id}");

            UpdateBasicUserInfo(user, model);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return ApiResponse.BadRequest("Cập nhật thông tin thất bại.", updateResult.Errors);

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                if (isSelfUpdate)
                {
                    if (string.IsNullOrWhiteSpace(model.CurrentPassword))
                        return ApiResponse.BadRequest("Bạn phải nhập mật khẩu hiện tại để đổi mật khẩu.");

                    var pwdResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    if (!pwdResult.Succeeded)
                        return ApiResponse.BadRequest("Đổi mật khẩu thất bại.", pwdResult.Errors);
                }
                else
                {
                    // Admin đổi hộ
                    await _userManager.RemovePasswordAsync(user);
                    var pwdResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
                    if (!pwdResult.Succeeded)
                        return ApiResponse.BadRequest("Đặt lại mật khẩu thất bại.", pwdResult.Errors);
                }
            }


            return ApiResponse.Success("Cập nhật thông tin thành công.");
        }



        private void UpdateBasicUserInfo(User user, UserBaseUpdateDTO model)
        {
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.BirthDate = model.BirthDate;
            user.Gender = model.Gender;
            user.Avatar = model.Avatar;
        }
        public static class ApiResponse
        {
            public static ApiResponseModel<object> Success(string msg) =>
                new(true, msg, null, 200);

            public static ApiResponseModel<object> NotFound(string msg) =>
                new(false, msg, null, 404);

            public static ApiResponseModel<object> BadRequest(string msg, IEnumerable<IdentityError>? errors = null) =>
                new(false, msg, null, 400, errors?.Select(e => e.Description).ToList());
        }

        public async Task<ApiResponseModel<object>> AddClaimAsync(string userId, string claimType, string claimValue)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ApiResponseModel<object>(
                    false, "Không tìm thấy người dùng.", null, 404);

            var claim = new Claim(claimType, claimValue);
            var result = await _userManager.AddClaimAsync(user, claim);
            if (!result.Succeeded)
                return new ApiResponseModel<object>(
                    false, "Thêm claim thất bại.", null, 400,
                    result.Errors.Select(e => e.Description).ToList());

            return new ApiResponseModel<object>(
                true, $"Đã thêm claim {claimType} = {claimValue} cho user {user.UserName}.", null, 200);
        }

        public async Task<ApiResponseModel<object>> ToggleUserStatusAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ApiResponseModel<object>(
                    isSuccess: false,
                    message: $"Không tìm thấy người dùng với Id: {userId}",
                    data: null,
                    statusCode: 404
                );
            }

            if (user.IsDisabled == true)
            {
                user.IsDisabled = false;
                user.DisabledDate = null;

                // Mở khoá lockout
                await _userManager.SetLockoutEnabledAsync(user, false);
                await _userManager.SetLockoutEndDateAsync(user, null);

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return new ApiResponseModel<object>(
                        isSuccess: true,
                        message: "Đã kích hoạt lại tài khoản thành công.",
                        data: null,
                        statusCode: 200
                    );
                }

                return new ApiResponseModel<object>(
                    isSuccess: false,
                    message: "Kích hoạt thất bại: " + string.Join("; ", result.Errors.Select(e => e.Description)),
                    data: null,
                    statusCode: 500
                );
            }
            else 
            {
                user.IsDisabled = true;
                user.DisabledDate = DateTime.Now;

                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return new ApiResponseModel<object>(
                        isSuccess: true,
                        message: "Đã vô hiệu hóa tài khoản thành công.",
                        data: null,
                        statusCode: 200
                    );
                }

                return new ApiResponseModel<object>(
                    isSuccess: false,
                    message: "Vô hiệu hóa thất bại: " + string.Join("; ", result.Errors.Select(e => e.Description)),
                    data: null,
                    statusCode: 500
                );
            }
        }
        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            return await Task.FromResult(_roleManager.Roles.ToList());
        }
    }
}
