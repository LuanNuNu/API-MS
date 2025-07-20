using MenShop_Assignment.Datas;
using MenShop_Assignment.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.Repositories
{
    public class AutoOrderRepository : IAutoOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public AutoOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetPendingOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Details)
                    .ThenInclude(d => d.ProductDetail)
                .Where(o => o.Status == OrderStatus.Pending)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderWithDetailsAsync(string orderId)
        {
            return await _context.Orders
                .Include(o => o.Details)
                    .ThenInclude(d => d.ProductDetail)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
