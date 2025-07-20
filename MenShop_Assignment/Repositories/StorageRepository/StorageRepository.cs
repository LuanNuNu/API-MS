using MenShop_Assignment.Datas;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using Microsoft.EntityFrameworkCore;

namespace MenShop_Assignment.Repositories.StorageRepositories
{
    public class StorageRepository : IStorageRepository
    {
        private readonly ApplicationDbContext _context;
        public StorageRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<StorageViewModel>> GetAllStoragesAsync()
        {
            var storages = await _context.Storages.Include(s => s.CategoryProduct).Include(s => s.Manager).Include(s => s.StorageDetails)
                    .ThenInclude(sd => sd.ProductDetail)
                        .ThenInclude(pd => pd.Product)
                    .ThenInclude(sd=>sd.ProductDetails)
                        .ThenInclude(pd => pd.Color)
					.ThenInclude(pd => pd.ProductDetails)
						.ThenInclude(pd => pd.Size)
                    .ThenInclude(sd=>sd.ProductDetails)
                        .ThenInclude(pd=> pd.Color)
					.ThenInclude(sd => sd.ProductDetails)
						.ThenInclude(pd => pd.Images)
				.ToListAsync();

            return storages.Select(StorageMapper.ToStorageViewModel).ToList();
        }
        public async Task<List<ProductViewModel>?> GetProductsByStorageIdAsync(int storageId)
        {
			
			var products = await _context.StorageDetails
				.Include(x => x.ProductDetail)
					.ThenInclude(pd => pd.Product)
				.Where(x => x.StorageId == storageId)
				.Select(x => x.ProductDetail.Product)
				.Distinct()
				.ToListAsync();
			if (!products.Any())
				return null;
			return products.Select(ProductMapper.ToProductViewModel).ToList();
		}
        public async Task<List<ProductDetailViewModel>?> GetDetailsByProductId(int productId)
        {
			var storageDetails = await _context.StorageDetails
							.Include(x => x.ProductDetail)
								.ThenInclude(pd => pd.Product)
							.Include(x => x.ProductDetail.Color)
							.Include(x => x.ProductDetail.Size)
							.Include(x => x.ProductDetail.Fabric)
							.Include(x => x.ProductDetail.Images)
							.Where(x =>x.ProductDetail.Product.ProductId == productId)
							.ToListAsync();
			if (storageDetails.Count == 0)
				return null;
			return storageDetails.Select(StorageMapper.ToStorageDetailViewModel).ToList();
		}

        public async Task<StorageDetail?> GetByProductIdAsync(int productDetailId)
        {
            return await _context.StorageDetails
                .FirstOrDefaultAsync(s => s.ProductDetailId == productDetailId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
