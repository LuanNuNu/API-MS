using Azure;
using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Extensions;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using MenShop_Assignment.Repositories.CartRepository;
using MenShop_Assignment.Services.PaymentServices;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.Repositories.OrderRepository
{
	public class OrderRepository : IOrderRepository
	{
		private readonly ApplicationDbContext _context;

		public OrderRepository(ApplicationDbContext context)
		{
			_context = context;
		}
		private IQueryable<Order> GetOrders()
		{
			return _context.Orders
				.Include(o => o.Customer)
				.Include(o => o.Employee)
				.Include(o => o.Shipper)
				.Include(o => o.Details)
					.ThenInclude(od => od.ProductDetail)
						.ThenInclude(pd => pd.Product)
				.Include(o => o.Details)
					.ThenInclude(od => od.ProductDetail)
						.ThenInclude(pd => pd.Size)
				.Include(o => o.Details)
					.ThenInclude(od => od.ProductDetail)
						.ThenInclude(pd => pd.Color)
				.Include(o => o.Details)
					.ThenInclude(od => od.ProductDetail)
						.ThenInclude(pd => pd.Fabric)
				.Include(o => o.Details)
					.ThenInclude(od => od.ProductDetail)
						.ThenInclude(pd => pd.Images)
				.AsSplitQuery();
		}
		public async Task<bool> CreateOrderAsync(CreateOrderDTO createProductDTO)
		{
			var viewModel = new OrderViewModel();
			var productDetailIds = createProductDTO.Details.Select(d => d.ProductDetailId).ToList();
			var orderDetails = createProductDTO.Details.Select(d => new OrderDetail
			{
				ProductDetailId = d.ProductDetailId,
				Quantity = d.Quantity,
				Price = d.Price
			}).ToList();
            bool isOnline = string.IsNullOrWhiteSpace(createProductDTO.EmployeeId);
            var order = new Order
			{
				OrderId = "OD" + DateTime.Now.ToString("yyyyMMddHHmmssfff"),
				CustomerId = createProductDTO.CustomerId,
                EmployeeId = isOnline ? null : createProductDTO.EmployeeId,
                ShipperId = createProductDTO.ShipperId,
                IsOnline = isOnline,
                CreatedDate = DateTime.Now,
				Status = OrderStatus.Pending,
				Total = orderDetails.Sum(d => (d.Price ?? 0) * d.Quantity).GetValueOrDefault(),
				PaidDate = null,
				CompletedDate = null,
				Address = null,
				Details = orderDetails
			};
			_context.Orders.Add(order);
			await _context.SaveChangesAsync();
			var cart = await _context.Carts.Include(c => c.Details).FirstOrDefaultAsync(c => c.CustomerId == createProductDTO.CustomerId);
			if (cart != null)
			{
				var detailsToRemove = cart.Details?.Where(cd => productDetailIds.Contains(cd.ProductDetailId ?? 0)).ToList();

				if (detailsToRemove != null && detailsToRemove.Any())
					_context.CartDetails.RemoveRange(detailsToRemove);

				if (cart.Details == null || cart.Details.Count == detailsToRemove?.Count)
					_context.Carts.Remove(cart);

				await _context.SaveChangesAsync();
			}
			var orderWithDetails = await _context.Orders
					.Include(o => o.Customer)
					.Include(o => o.Employee)
					.Include(o => o.Shipper)
					.Include(o => o.Payments).ThenInclude(p => p.Discounts)
					.AsSplitQuery()
					.FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
			if (orderWithDetails == null)
				return false;

			return true;
		}
		public async Task<List<OrderViewModel>> GetOrdersAsync(SearchOrderDTO? search)
		{
			var orderList = await GetOrders().ToListAsync();
			var validDistricts = new[] { "Liên Chiểu", "Hải Châu", "Ngũ Hành Sơn", "Sơn Trà", "Cẩm Lệ", "Thanh Khê" };
			if (search == null)
				return orderList.Select(OrderMapper.ToOrderViewModel).ToList();
			if (search.CustomerId != null)
				orderList = orderList.Where(x => x.CustomerId == search.CustomerId).ToList();
			if (search.OrderId != null)
				orderList = orderList.Where(x => x.OrderId == search.OrderId).ToList();
			if (search.ShipperId != null)
				orderList = orderList.Where(x => x.ShipperId == search.ShipperId).ToList();
			if (search.Status != null)
				orderList = orderList.Where(x => x.Status == search.Status).ToList();
			if (search.District != null)
				if (!validDistricts.Contains(search.District))
					throw new ArgumentException($"Quận/huyện không hợp lệ. Chỉ chấp nhận: {string.Join(", ", validDistricts)}");
			orderList = orderList.Where(x => x.Address != null && x.Address.ToLower().Contains(search.District.ToLower())).ToList();
			if (search.StartDate != null && search.EndDate != null)
				orderList = orderList.Where(x => x.CompletedDate >= search.StartDate && x.CompletedDate <= search.EndDate).ToList();
			if (search.IsOnline != null)
				if (search.IsOnline.Value)
					orderList.Where(x => x.IsOnline.Value).ToList();
				else
					orderList.Where(x => !x.IsOnline.Value).ToList();
			return orderList.Select(OrderMapper.ToOrderViewModel).ToList()?? [];
		}
		public async Task<bool> ShipperAcceptOrderByOrderId(string orderId, string shipperId)
		{
			var order = await GetOrders().FirstOrDefaultAsync(x => x.OrderId == orderId);

			if (order == null)
				return false;

			order.ShipperId = shipperId;
			order.Status = OrderStatus.Delivering;
			_context.Entry(order).State = EntityState.Modified;
			await _context.SaveChangesAsync();
			return true;
		}
		public async Task<bool> CompletedOrderStatus(string orderId)
		{
			var order = GetOrders().FirstOrDefault(x => x.OrderId == orderId);
			if (order == null)
				return false;

			order.Status = OrderStatus.Completed;
			await _context.SaveChangesAsync();
			return true;
		}
		public async Task<bool> CancelOrderAsync(string orderId)
		{
			var order = await _context.Orders
								.Include(o => o.Details)
								.AsSplitQuery()
								.FirstOrDefaultAsync(o => o.OrderId == orderId);

			if (order == null)
				return false;

			if (!(order.Status == OrderStatus.Pending || order.Status == OrderStatus.Confirmed))
				return false;

			order.Status = OrderStatus.Cancelled;
			await _context.SaveChangesAsync();

			if (order.Details != null && order.Details.Any())
			{
				foreach (var detail in order.Details)
				{
					var branchDetail = await _context.BranchDetails.Include(bd => bd.Branch).Where(bd => bd.Branch.Address.ToLower().Contains("online"))
						.FirstOrDefaultAsync(sd => sd.ProductDetailId == detail.ProductDetailId);

					if (branchDetail != null && detail.Quantity.HasValue)
					{
						branchDetail.Quantity = (branchDetail.Quantity ?? 0) + detail.Quantity.Value;
						await _context.SaveChangesAsync();
					}
				}
			}
			return true;
		}
	}
}
