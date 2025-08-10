using MenShop_Assignment.Datas;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Mapper
{
    public static class StorageMapper
    {
        public static StorageViewModel ToStorageViewModel(Storage storage)
        {
            return new StorageViewModel
            {
                StorageId = storage.StorageId,
                CategoryName = storage.CategoryProduct?.Name ?? null,
                ManagerName = storage.Manager?.FullName ?? null
            };
        }
        public static ProductDetailViewModel ToStorageDetailViewModel(StorageDetail storage)
        {
            return new ProductDetailViewModel
            {
				DetailId = storage.ProductDetailId,
				ProductName = storage.ProductDetail?.Product?.ProductName ?? null,
				SizeName = storage.ProductDetail?.Size?.Name ?? null,
				ColorName = storage.ProductDetail?.Color?.Name ?? null,
				FabricName = storage.ProductDetail?.Fabric?.Name ?? null,
				Quantity = storage.Quantity ?? null,
				SellPrice = storage.Price ?? null,
				Images = storage.ProductDetail?.Images?.Select(x => x.FullPath).ToList() ?? [],
			};
        }
    }
}
