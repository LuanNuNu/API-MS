using MenShop_Assignment.Datas;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Repositories.StorageRepositories
{
    public interface IStorageRepository
    {
        Task<List<StorageViewModel>> GetAllStoragesAsync();
		Task<List<ProductDetailViewModel>> GetDetailsByProductId(int productId);
        Task<List<ProductViewModel>> GetProductsByStorageIdAsync(int storageId);
        Task<StorageDetail?> GetByProductIdAsync(int productDetailId);
        Task SaveChangesAsync();
    }
}
