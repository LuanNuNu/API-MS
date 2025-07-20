using MenShop_Assignment.Datas;
using MenShop_Assignment.Models;
using MenShop_Assignment.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MenShop_Assignment.Repositories.CartRepository
{
    public interface ICartRepository
    {
		Task<CartViewModel?> GetCartByCustomerIdAsync(string customerId);
		Task<bool> AddToCartAsync(string customerId, int productDetailId, int quantity);
		Task<bool> RemoveFromCartAsync(string customerId, int productDetailId);
		Task<bool> UpdateQuantityAsync(string customerId, int productDetailId, int quantity);
	}
}
