using MenShop_Assignment.Datas;
using MenShop_Assignment.Extensions;
using MenShop_Assignment.Models;
using MenShop_Assignment.Models.Statistics;
using MenShop_Assignment.Repositories.StatisticRepository;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.Repositories.StatisticRepository
{
    public class StatisticRepository : IStatisticRepository
    {
        private readonly ApplicationDbContext _context;
        public StatisticRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResponseModel<List<DynamicStatisticItem>>> GetDynamicStatisticsAsync(DynamicStatisticRequest request)
        {
            try
            {
                var query = _context.Orders
                    .Where(o => o.Status == OrderStatus.Completed && o.CreatedDate != null)
                    .Include(o => o.Details).AsQueryable();

                if (request.BranchId.HasValue)
                {
                    query = query.Where(o => o.BranchId == request.BranchId);
                }

                if (request.Mode == StatisticMode.Day)
                {
                    if (!request.Year.HasValue || !request.Month.HasValue)
                        return new ApiResponseModel<List<DynamicStatisticItem>>(false, "Cần phải có năm và tháng khi thống kê theo ngày", null, 400);

                    query = query.Where(o => o.CreatedDate!.Value.Year == request.Year && o.CreatedDate!.Value.Month == request.Month);

                    var result = await query
                        .GroupBy(o => o.CreatedDate!.Value.Day)
                        .OrderBy(g => g.Key)
                        .Select(g => new DynamicStatisticItem
                        {
                            Label = $"Ngày {g.Key}",
                            OrderCount = g.Count(),
                            ProductSold = g.SelectMany(o => o.Details).Sum(d => d.Quantity ?? 0),
                            Revenue = g.Sum(o => (o.Subtotal ?? 0) + (o.ShippingFee ?? 0))
                        })
                        .ToListAsync();

                    return new ApiResponseModel<List<DynamicStatisticItem>>(true, "Lấy dữ liệu thành công", result, 200);
                }

                if (request.Mode == StatisticMode.Month)
                {
                    if (!request.Year.HasValue)
                        return new ApiResponseModel<List<DynamicStatisticItem>>(false, "Cần phải có năm khi thống kê theo tháng", null, 400);

                    query = query.Where(o => o.CreatedDate!.Value.Year == request.Year);

                    var result = await query
                        .GroupBy(o => o.CreatedDate!.Value.Month)
                        .OrderBy(g => g.Key)
                        .Select(g => new DynamicStatisticItem
                        {
                            Label = $"Tháng {g.Key}",
                            OrderCount = g.Count(),
                            ProductSold = g.SelectMany(o => o.Details).Sum(d => d.Quantity ?? 0),
                            Revenue = g.Sum(o => (o.Subtotal ?? 0) + (o.ShippingFee ?? 0))
                        })
                        .ToListAsync();

                    return new ApiResponseModel<List<DynamicStatisticItem>>(true, "Lấy dữ liệu thành công", result, 200);
                }

                var yearResult = await query
                    .GroupBy(o => o.CreatedDate!.Value.Year)
                    .OrderBy(g => g.Key)
                    .Select(g => new DynamicStatisticItem
                    {
                        Label = $"Năm {g.Key}",
                        OrderCount = g.Count(),
                        ProductSold = g.SelectMany(o => o.Details).Sum(d => d.Quantity ?? 0),
                        Revenue = g.Sum(o => (o.Subtotal ?? 0) + (o.ShippingFee ?? 0))
                    })
                    .ToListAsync();

                return new ApiResponseModel<List<DynamicStatisticItem>>(true, "Lấy dữ liệu thành công", yearResult, 200);
            }
            catch (Exception ex)
            {
                return new ApiResponseModel<List<DynamicStatisticItem>>(false, "Đã xảy ra lỗi trong quá trình xử lý: " + ex.Message, null, 500);
            }
        }

        public async Task<ApiResponseModel<List<object>>> GetTopBestSellingProductsAsync(int top = 10)
        {
            try
            {
                var topProducts = await _context.OrderDetails
                    .GroupBy(od => od.ProductDetail!.Product!.ProductName)
                    .Select(g => new
                    {
                        ProductName = g.Key,
                        TotalQuantity = g.Sum(x => x.Quantity ?? 0)
                    })
                    .OrderByDescending(x => x.TotalQuantity)
                    .Take(top)
                    .ToListAsync();

                return new ApiResponseModel<List<object>>(true, "Lấy danh sách sản phẩm bán chạy thành công", topProducts.Cast<object>().ToList(), 200);
            }
            catch (Exception ex)
            {
                return new ApiResponseModel<List<object>>(false, "Đã xảy ra lỗi trong quá trình xử lý: " + ex.Message, null, 500);
            }
        }

        public async Task<ApiResponseModel<List<object>>> GetTopCustomersAsync(int top = 10)
        {
            try
            {
                var topCustomers = await _context.Orders
                    .Where(o => o.Customer != null)
                    .GroupBy(o => o.Customer!.FullName)
                    .Select(g => new
                    {
                        CustomerName = g.Key,
                        TotalOrders = g.Count(),
                        TotalSpent = g.Sum(o => (o.Subtotal ?? 0) + (o.ShippingFee ?? 0))
                    })
                    .OrderByDescending(x => x.TotalSpent)
                    .Take(top)
                    .ToListAsync();

                return new ApiResponseModel<List<object>>(true, "Lấy danh sách khách hàng tiêu biểu thành công", topCustomers.Cast<object>().ToList(), 200);
            }
            catch (Exception ex)
            {
                return new ApiResponseModel<List<object>>(false, "Đã xảy ra lỗi trong quá trình xử lý: " + ex.Message, null, 500);
            }
        }


    }
}
