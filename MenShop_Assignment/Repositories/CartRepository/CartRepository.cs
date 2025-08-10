using Castle.Core.Resource;
using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using MenShop_Assignment.Models.Account;
using MenShop_Assignment.Models.VNPay;
using MenShop_Assignment.Repositories.CartRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MenShop_Assignment.Repositories.CartRepository
{
	public class CartRepository : ICartRepository
	{
		private readonly ApplicationDbContext _context;

		public CartRepository(ApplicationDbContext context)
		{
			_context = context;
		}

        private IQueryable<Cart> GetCartWithDetails()
        {
            return _context.Carts
                .Include(c => c.Details)
                    .ThenInclude(cd => cd.ProductDetail)
                        .ThenInclude(pd => pd.Product)
                .Include(c => c.Details)
                    .ThenInclude(cd => cd.ProductDetail.Color)
                .Include(c => c.Details)
                    .ThenInclude(cd => cd.ProductDetail.Size)
                .Include(c => c.Details)
                    .ThenInclude(cd => cd.ProductDetail.Fabric)
                .Include(c => c.Details)
                    .ThenInclude(cd => cd.ProductDetail.Images)
                .Include(c => c.Customer)
                .AsSplitQuery()
                .AsNoTracking();
        }

        public async Task<CartViewModel?> GetCartByCustomerIdAsync(string customerId)
        {
            var cart = await GetCartWithDetails()
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            return cart == null ? null : CartMapper.ToCartViewModel(cart);
        }

        public async Task<CartViewModel?> GetCartByAnonymousIdAsync(string anonymousId)
        {
            var cart = await GetCartWithDetails()
                .FirstOrDefaultAsync(c => c.AnonymousId == anonymousId);

            return cart == null ? null : CartMapper.ToCartViewModel(cart);
        }

		private async Task<Branch> GetOnlineBranchAsync()
		{
			return await _context.Branches
				.Include(b => b.BranchDetails)
				.FirstOrDefaultAsync(b => b.IsOnline == true);
		}

        public async Task<Cart?> GetCartAsync(string? customerId, string? anonymousId)
        {
            IQueryable<Cart> query = _context.Carts
                .Include(c => c.Details)
                .ThenInclude(d => d.ProductDetail);

            if (!string.IsNullOrEmpty(customerId))
            {
                return await query.FirstOrDefaultAsync(c => c.CustomerId == customerId);
            }
            else if (!string.IsNullOrEmpty(anonymousId))
            {
                return await query.FirstOrDefaultAsync(c => c.AnonymousId == anonymousId);
            }

            return null;
        }

        public async Task<bool> AddToCartAsync(string? customerId, string? anonymousId, int productDetailId, int quantity)
        {
            var branch = await GetOnlineBranchAsync();
            if (branch == null)
                return false;

            var cart = await GetCartAsync(customerId, anonymousId);
            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = customerId,
                    AnonymousId = anonymousId,
                    CreatedDate = DateTime.Now,
                    Details = new List<CartDetail>()
                };
                await _context.Carts.AddAsync(cart);
            }

            var existingItem = cart.Details.FirstOrDefault(d => d.ProductDetailId == productDetailId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var productDetailExists = await _context.ProductDetails
                    .AnyAsync(pd => pd.DetailId == productDetailId);

                if (!productDetailExists)
                    return false;

                cart.Details.Add(new CartDetail
                {
                    ProductDetailId = productDetailId,
                    Quantity = quantity,
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromCartAsync(string customerId, string? anonymousId, int productDetailId)
		{
            var cart = await GetCartAsync(customerId, anonymousId);
            if (cart == null) 
				return false;

			var item = cart.Details.FirstOrDefault(d => d.ProductDetailId == productDetailId);
			if (item == null) 
				return false;

			cart.Details.Remove(item);
            if (!cart.Details.Any())
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> UpdateQuantityAsync(string customerId, string? anonymousId, int productDetailId, int quantity)
		{
			if (quantity < 1) 
				return false;

            var cart = await GetCartAsync(customerId, anonymousId);

            if (cart == null) 
				return false;

			var item = cart.Details.FirstOrDefault(d => d.ProductDetailId == productDetailId);
			if (item == null) 
				return false;

			item.Quantity = quantity;
			await _context.SaveChangesAsync();
			return true;
		}

        public async Task AssignCartToCustomerAsync(int cartId, string customerId)
        {
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart == null) return;

            cart.CustomerId = customerId;
            cart.AnonymousId = null;
            await _context.SaveChangesAsync();
        }
        public async Task MergeItemAsync(int customerCartId, CartDetailViewModel anonymousItem)
        {
            var customerCart = await _context.Carts
                .Include(c => c.Details)
                .FirstOrDefaultAsync(c => c.CartId == customerCartId);

            if (customerCart == null) return;

            int productDetailId = anonymousItem.DetailId;
            int quantity = anonymousItem.Quantity ?? 1;

            var existingItem = customerCart.Details
                .FirstOrDefault(d => d.ProductDetailId == productDetailId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {

                customerCart.Details.Add(new CartDetail
                {
                    ProductDetailId = productDetailId,
                    Quantity = quantity
                });
            }

            await _context.SaveChangesAsync();
        }



        public async Task DeleteCartByAnonymousIdAsync(string anonymousId)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.AnonymousId == anonymousId);

            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }

    }
}
