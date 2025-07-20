using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Extensions;
using MenShop_Assignment.Models;
using MenShop_Assignment.Repositories;
using MenShop_Assignment.Repositories.OrderRepository;
using MenShop_Assignment.Repositories.StorageRepositories;
namespace MenShop_Assignment.Services
{
    public class AutoOrderService : IAutoOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IStorageRepository _storageRepo;
        private readonly ApplicationDbContext _context;

        public AutoOrderService(IOrderRepository orderRepo, IStorageRepository storageRepo,ApplicationDbContext context)
        {
            _orderRepo = orderRepo;
            _storageRepo = storageRepo;
            _context = context;
        }

        public async Task<ApprovalResultDto> ApproveOrderAsync(string orderId)
        {
            var order =  _orderRepo.GetOrdersAsync(new SearchOrderDTO { OrderId = orderId}).Result.FirstOrDefault();

            if (order == null)
                return new ApprovalResultDto { Success = false, OrderId = orderId, Message = "Không tìm thấy đơn hàng" };

            if (order.Status != OrderStatus.Pending.ToString())
                return new ApprovalResultDto { Success = false, OrderId = orderId, Message = "Đơn hàng không ở trạng thái chờ xử lý" };
            if (!order.IsOnline.Contains("Online"))
                return new ApprovalResultDto { Success = false, OrderId = orderId, Message = "Đơn hàng không là đơn hàng Online" };

            foreach (var detail in order.Details)
            {
                var storage = await _storageRepo.GetByProductIdAsync(detail.DetailId);
                if (storage == null)
                    return new ApprovalResultDto { Success = false, OrderId = orderId, Message = $"Sản phẩm {detail.DetailId} không có trong kho" };

                if (storage.Quantity < detail.Quantity)
                    return new ApprovalResultDto { Success = false, OrderId = orderId, Message = $"Sản phẩm {detail.DetailId} không đủ hàng" };
            }

            foreach (var detail in order.Details)
            {
                var storage = await _storageRepo.GetByProductIdAsync(detail.DetailId);
                storage.Quantity -= detail.Quantity;
            }

            order.Status = OrderStatus.Confirmed.ToString();
            order.CompletedDate = DateTime.Now;

            _context.SaveChanges();

            return new ApprovalResultDto { Success = true, OrderId = orderId, Message = "Duyệt đơn hàng thành công" };
        }

        public async Task<BatchApprovalDto> ApproveAllOrdersAsync()
        {
            var onlinePendingOrders =  _orderRepo.GetOrdersAsync(new SearchOrderDTO { Status = OrderStatus.Pending, IsOnline=true}).Result.ToList();


            var result = new BatchApprovalDto { Total = onlinePendingOrders.Count };

            foreach (var order in onlinePendingOrders)
            {
                var approvalResult = await ApproveOrderAsync(order.OrderId);
                if (approvalResult.Success)
                {
                    result.Approved++;
                    result.ApprovedIds.Add(order.OrderId);
                }
                else
                {
                    result.Failed++;
                    result.FailedOrders.Add(new FailedOrderDto
                    {
                        OrderId = order.OrderId,
                        Error = approvalResult.Message
                    });
                }
            }

            return result;
        }

        public async Task<PendingOrdersDto> GetPendingOrdersAsync()
        {
            var orders = _orderRepo.GetOrdersAsync(new SearchOrderDTO { Status = OrderStatus.Pending }).Result;

            return new PendingOrdersDto
            {
                Count = orders.Count,
                Orders = orders.Select(order => new OrderViewModel
                {
                    OrderId = order.OrderId,
                    CustomerName = order.CustomerName,
                    EmployeeName = order.EmployeeName,
                    ShipperName = order.ShipperName,
                    CreatedDate = order.CreatedDate,
                    CompletedDate = order.CompletedDate,
                    PaidDate = order.PaidDate,
                    Status = order.Status.ToString(),
                    IsOnline = order.IsOnline,
                    Total = order.Total,
                }).ToList()
            };
        }

        
    }
}