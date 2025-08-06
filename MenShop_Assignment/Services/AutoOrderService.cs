using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Extensions;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using MenShop_Assignment.Repositories;
using MenShop_Assignment.Repositories.BranchesRepository;
using MenShop_Assignment.Repositories.OrderRepository;
using MenShop_Assignment.Repositories.StorageRepositories;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace MenShop_Assignment.Services
{
    public class AutoOrderService : IAutoOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IStorageRepository _storageRepo;
        private readonly IBranchRepository _branchRepo;
        private readonly ApplicationDbContext _context;

        public AutoOrderService(IOrderRepository orderRepo, IStorageRepository storageRepo, IBranchRepository branchRepo, ApplicationDbContext context)
        {
            _orderRepo = orderRepo;
            _storageRepo = storageRepo;
            _branchRepo = branchRepo;
            _context = context;
        }

        private async Task<ApprovalResultDto> ApproveOrderBaseAsync(string orderId, int? branchId)
        {
            var orderDetails = await _orderRepo.GetOrderDetailsByOrderIdsAsync(orderId);

            foreach (var detail in orderDetails)
            {
                var branchStorage = await _context.BranchDetails
                    .FirstOrDefaultAsync(x => x.ProductDetailId == detail.DetailId && x.BranchId == branchId);

                if (branchStorage == null)
                {
                    return new ApprovalResultDto
                    {
                        Success = false,
                        OrderId = orderId,
                        Message = $"Không tồn tại sản phẩm mã {detail.DetailId}"
                    };
                }

                if (branchStorage.Quantity < detail.Quantity)
                {
                    return new ApprovalResultDto
                    {
                        Success = false,
                        OrderId = orderId,
                        Message = $"Mã sản phẩm = {detail.DetailId} hiện không đủ số lượng!"
                    };
                }

                branchStorage.Quantity -= detail.Quantity;
                _context.Update(branchStorage);
            }

            await _context.SaveChangesAsync();

            var orderEntity = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (orderEntity == null)
            {
                return new ApprovalResultDto { Success = false, OrderId = orderId, Message = "Không tìm thấy đơn hàng trong CSDL" };
            }

            orderEntity.Status = OrderStatus.Confirmed;
            _context.Update(orderEntity);
            await _context.SaveChangesAsync();

            return new ApprovalResultDto
            {
                Success = true,
                OrderId = orderId,
                Message = "Duyệt đơn hàng thành công"
            };
        }

        public async Task<ApprovalResultDto> ApproveOnlineOrderAsync(string orderId)
        {
            var orderVm = await _orderRepo.GetOrdersAsync(new SearchOrderDTO { OrderId = orderId })
                                          .ContinueWith(t => t.Result.FirstOrDefault());

            if (orderVm == null)
                return new ApprovalResultDto { Success = false, OrderId = orderId, Message = "Không tìm thấy đơn hàng" };

            if (orderVm.Status != OrderStatus.Pending)
                return new ApprovalResultDto { Success = false, OrderId = orderId, Message = "Đơn hàng không ở trạng thái chờ xử lý" };

            if (string.IsNullOrEmpty(orderVm.IsOnline) || !orderVm.IsOnline.Contains("Online"))
                return new ApprovalResultDto { Success = false, OrderId = orderId, Message = "Đơn hàng không phải là đơn hàng Online" };

            var branchOnline = (await _branchRepo.GetBranchesAsync())
                                .FirstOrDefault(b => b.IsOnline == true);

            if (branchOnline == null)
                return new ApprovalResultDto { Success = false, OrderId = orderId, Message = "Không tìm thấy chi nhánh online" };

            return await ApproveOrderBaseAsync(orderId, branchOnline.BranchId);
        }

        public async Task<ApprovalResultDto> ApproveOfflineOrderAsync(string orderId)
        {
            var orderVm = await _orderRepo.GetOrdersAsync(new SearchOrderDTO { OrderId = orderId })
                                          .ContinueWith(t => t.Result.FirstOrDefault());

            if (orderVm == null)
                return new ApprovalResultDto { Success = false, OrderId = orderId, Message = "Không tìm thấy đơn hàng!" };

            if (orderVm.Status != OrderStatus.Completed)
                return new ApprovalResultDto { Success = false, OrderId = orderId, Message = "Đơn hàng hiện chưa hoàn tất!" };

            if (orderVm.BranchId == null)
                return new ApprovalResultDto { Success = false, OrderId = orderId, Message = "Không tìm thấy chi nhánh của đơn hàng!" };

            return await ApproveOrderBaseAsync(orderId, orderVm.BranchId);
        }

    }
}