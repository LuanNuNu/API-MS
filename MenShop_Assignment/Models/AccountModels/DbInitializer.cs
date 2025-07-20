using MenShop_Assignment.Datas;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MenShop_Assignment.Models.Account
{
    public static class DbInitializer
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            string adminEmail = config["AdminSeed:Email"];
            string configPassword = config["AdminSeed:Password"];
            bool.TryParse(config["AdminSeed:UseRandomPassword"], out bool useRandomPassword);

            string adminPassword = useRandomPassword
                ? "Admin@" + Guid.NewGuid().ToString("N").Substring(0, 8)
                : configPassword;

            string roleName = "Admin";

            try
            {
                // Tạo vai trò nếu chưa có
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (roleResult.Succeeded)
                        logger.LogInformation(" Đã tạo vai trò '{Role}'.", roleName);
                    else
                        logger.LogError("Không thể tạo vai trò '{Role}': {Errors}",
                            roleName,
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }

                // Kiểm tra user admin
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    var newAdmin = new User
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        FullName = "Quản trị viên"
                    };

                    var createUserResult = await userManager.CreateAsync(newAdmin, adminPassword);

                    if (createUserResult.Succeeded)
                    {
                        logger.LogInformation("Tài khoản admin đã được tạo thành công.");
                        if (useRandomPassword)
                            logger.LogInformation("Mật khẩu ngẫu nhiên: {Password}", adminPassword);

                        var addToRoleResult = await userManager.AddToRoleAsync(newAdmin, roleName);
                        if (addToRoleResult.Succeeded)
                            logger.LogInformation("Đã gán vai trò '{Role}' cho tài khoản admin.", roleName);
                        else
                            logger.LogError("Không thể gán vai trò cho admin: {Errors}",
                                string.Join(", ", addToRoleResult.Errors.Select(e => e.Description)));
                    }
                    else
                    {
                        logger.LogError("Không thể tạo tài khoản admin: {Errors}",
                            string.Join(", ", createUserResult.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger.LogWarning("Tài khoản admin đã tồn tại.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi xảy ra khi khởi tạo dữ liệu admin.");
            }
        }
    }
}
