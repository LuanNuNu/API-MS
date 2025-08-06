using MenShop_Assignment.Datas;
using MenShop_Assignment.Models;
using MenShop_Assignment.Models.Account;
using MenShop_Assignment.Models.VNPay;
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
        Task<CartViewModel?> GetCartByAnonymousIdAsync(string anonymousId);
        Task<bool> AddToCartAsync(string? customerId, string? anonymousId, int productDetailId, int quantity);
		Task<bool> RemoveFromCartAsync(string customerId, string? anonymousId, int productDetailId);
		Task<bool> UpdateQuantityAsync(string customerId, string? anonymousId, int productDetailId, int quantity);

        Task AssignCartToCustomerAsync(int cartId, string customerId);
        Task MergeItemAsync(int customerCartId, CartDetailViewModel anonymousItem);
        Task DeleteCartByAnonymousIdAsync(string anonymousId);

    }
}
