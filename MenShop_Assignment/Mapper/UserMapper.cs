using MenShop_Assignment.Datas;
using MenShop_Assignment.Models;
using Microsoft.AspNetCore.Identity;

namespace MenShop_Assignment.Mapper
{
    public static class UserMapper
    {
        public static UserViewModel MapToViewModel(User user, IList<string> roles, IList<System.Security.Claims.Claim> claims)
        {
            return new UserViewModel
            {
                UserId = user.Id,
                FullName = user.FullName,
                Avatar = user.Avatar,
                UserEmail = user.Email,
                UserPhone = user.PhoneNumber,
                UserRole = roles.FirstOrDefault(), 
                Gender = user.Gender?.ToString(), 
                BirthDate = user.BirthDate,
                WorkedBranch = user.EmployeeAddress, 
                ManagerName = user.ManagerId, 
                CreatedDate = user.CreatedDate,
                DisabledDate = user.DisabledDate,
                IsDisabled = user.IsDisabled,
                BranchId = user.BranchId,
                Claims = claims.Select(c => new ClaimViewModel
                {
                    Type = c.Type,
                    Value = c.Value
                }).ToList()
            };
        }


    }
}
