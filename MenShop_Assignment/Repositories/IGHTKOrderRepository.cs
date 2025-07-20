using MenShop_Assignment.Models;
using MenShop_Assignment.Models.GHTKModel;

namespace MenShop_Assignment.Repositories
{
    public interface IGHTKOrderRepository
    {
        Task<List<GHTKOrderViewModel>> GetAllGHTKOrdersAsync();
    }
}
