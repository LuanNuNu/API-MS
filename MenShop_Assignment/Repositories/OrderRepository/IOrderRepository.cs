using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Repositories.OrderRepository
{
	public interface IOrderRepository
	{
		Task<bool> CancelOrderAsync(string orderId);
		Task<bool> CompletedOrderStatus(string orderId);
		Task<bool> CreateOrderAsync(CreateOrderDTO createProductDTO);
		Task<List<OrderViewModel>> GetOrdersAsync(SearchOrderDTO? search);
		Task<bool> ShipperAcceptOrderByOrderId(string orderId, string shipperId);
	}
}