using MenShop_Assignment.DTOs;
namespace MenShop_Assignment.Services
{
    public interface IAutoOrderService
    {
        Task<ApprovalResultDto> ApproveOfflineOrderAsync(string orderId);
        Task<ApprovalResultDto> ApproveOnlineOrderAsync(string orderId);
    }
}
