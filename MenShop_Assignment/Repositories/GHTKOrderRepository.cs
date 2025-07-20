using MenShop_Assignment.Datas;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using MenShop_Assignment.Models.GHTKModel;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.Repositories
{
    public class GHTKOrderRepository : IGHTKOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly GHTKOrderMapper _ordersMapper;
        public GHTKOrderRepository(ApplicationDbContext context, GHTKOrderMapper ordersMapper)
        {
            _context = context;
            _ordersMapper = ordersMapper;
        }
        public async Task<List<GHTKOrderViewModel>> GetAllGHTKOrdersAsync()
        {
            // Load tất cả GHTKOrders từ database
            var orders = await _context.GHTKOrders.ToListAsync();

            // Map từng order một cách async
            var viewModels = new List<GHTKOrderViewModel>();
            foreach (var order in orders)
            {
                var viewModel = await _ordersMapper.ToGHTKOrderView(order);
                viewModels.Add(viewModel);
            }

            return viewModels;
        }
    }
}
