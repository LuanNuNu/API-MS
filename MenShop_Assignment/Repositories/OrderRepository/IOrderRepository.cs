using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Repositories.OrderRepository
{
	public interface IOrderRepository
	{
        Task<ApiResponseModel<object>> CancelOrderAsync(string orderId, string reason);
        Task<ApiResponseModel<object>> CompletedOrderStatus(string orderId);
        Task<OrderResponseDTO> CreateOrderAsync(CreateOrderDTO createOrderDTO);
		Task<List<OrderViewModel>> GetOrdersAsync(SearchOrderDTO? search);
        Task<List<OrderProductDetailViewModel>> GetOrderDetailsByOrderIdsAsync(string orderId);
        Task<ApiResponseModel<object>> ShipperAcceptOrderByOrderId(string orderId, string shipperId);
        Task<ApprovalResultDto> RestoreStockForCancelledOrderAsync(string orderId);
    }
}