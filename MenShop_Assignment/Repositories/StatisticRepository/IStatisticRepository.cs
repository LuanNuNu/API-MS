using MenShop_Assignment.Models;
using MenShop_Assignment.Models.Statistics;

namespace MenShop_Assignment.Repositories.StatisticRepository
{
    public interface IStatisticRepository
    {
        Task<ApiResponseModel<List<DynamicStatisticItem>>> GetDynamicStatisticsAsync(DynamicStatisticRequest request);
        Task<ApiResponseModel<List<object>>> GetTopBestSellingProductsAsync(int top = 10);
        Task<ApiResponseModel<List<object>>> GetTopCustomersAsync(int top = 10);
        Task<ApiResponseModel<List<object>>>
            GetTopBestSellingProductsByDayAsync(DateTime? date = null, int top = 10, int? branchId = null);
        Task<ApiResponseModel<int>> GetTotalOrdersByDayAsync(DateTime? date = null, int? branchId = null);
    }
}
