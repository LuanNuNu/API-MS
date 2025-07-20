using MenShop_Assignment.Datas;

namespace MenShop_Assignment.Repositories
{
    public interface IAutoOrderRepository
    {
        Task<List<Order>> GetPendingOrdersAsync();
        Task<Order?> GetOrderWithDetailsAsync(string orderId);
        Task SaveChangesAsync();
    }
}
