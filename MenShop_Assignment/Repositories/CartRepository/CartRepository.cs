using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using MenShop_Assignment.Models.Account;
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

		public async Task<CartViewModel?> GetCartByCustomerIdAsync(string customerId)
		{
			var cart = await _context.Carts
				.Include(c => c.Customer)
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
				.FirstOrDefaultAsync(c => c.CustomerId == customerId);
			return cart == null ? null : CartMapper.ToCartViewModel(cart);
		}
		public async Task<bool> AddToCartAsync(string customerId, int productDetailId, int quantity)
		{
			var branch = await _context.Branches.Include(b => b.BranchDetails).Where(b => b.Address.Contains("Online")).FirstOrDefaultAsync();
			if (branch == null)
				return false;
			var cart = await _context.Carts
				.Include(c => c.Details)
				.FirstOrDefaultAsync(c => c.CustomerId == customerId);
			if (cart == null)
			{
				cart = new Cart
				{
					CustomerId = customerId,
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
				var productDetail = await _context.ProductDetails
					.Include(pd => pd.Product)
					.FirstOrDefaultAsync(pd => pd.DetailId == productDetailId);
				if (productDetail == null) 
					return false;
				cart.Details.Add(new CartDetail
				{
					ProductDetailId = productDetailId,
					Quantity = quantity,
					Price = branch.BranchDetails.Where(x=>x.ProductDetailId==productDetailId).FirstOrDefault()?.Price,
				});
			}
			await _context.SaveChangesAsync();
			return true;
		}
		public async Task<bool> RemoveFromCartAsync(string customerId, int productDetailId)
		{
			var cart = await _context.Carts
				.Include(c => c.Details)
				.FirstOrDefaultAsync(c => c.CustomerId == customerId);
			if (cart == null) 
				return false;

			var item = cart.Details.FirstOrDefault(d => d.ProductDetailId == productDetailId);
			if (item == null) 
				return false;

			cart.Details.Remove(item);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> UpdateQuantityAsync(string customerId, int productDetailId, int quantity)
		{
			if (quantity < 1) 
				return false;

			var cart = await _context.Carts
				.Include(c => c.Details)
				.FirstOrDefaultAsync(c => c.CustomerId == customerId);

			if (cart == null) 
				return false;

			var item = cart.Details.FirstOrDefault(d => d.ProductDetailId == productDetailId);
			if (item == null) 
				return false;

			item.Quantity = quantity;
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
