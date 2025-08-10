using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using MenShop_Assignment.Models.ProductModels.ReponseDTO;

namespace MenShop_Assignment.Mapper
{
	public static class ProductMapper
	{
        public static ProductViewModel ToProductViewModel(Product product)
        {
            return new ProductViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CategoryProductID = product.Category?.CategoryId,
                CategoryName = product.Category?.Name,
                Description = product.Description,
                Status = product.Status.ToString(),
                Thumbnail = product.ProductDetails?.FirstOrDefault()?.Images?.FirstOrDefault()?.FullPath ?? null,
                ProductDetails = product.ProductDetails?.Select(ToProductDetailViewModel)?.ToList() ?? []
            };
        }

        //
		public static ProductViewModel ToBranchProductViewModel(Product product, BranchDetail? branchDetail)
		{
            var firstCollection = product.CollectionDetails?.FirstOrDefault()?.Collection;
            return new ProductViewModel
			{
				ProductId = product.ProductId,
				ProductName = product.ProductName,
				CategoryProductID = product.Category?.CategoryId,
                CollectionId = firstCollection?.CollectionId,
                CollectionName = firstCollection?.CollectionName,
                Status = product.Status.ToString(),
				Price = branchDetail?.Price, 
				Thumbnail = branchDetail?.ProductDetail?.Images?.FirstOrDefault()?.FullPath
			};
		}


        public static ProductDetailViewModel ToProductDetailViewModel(ProductDetail productDetail)
        {
            var latestPrice = productDetail.HistoryPrices?
                .OrderByDescending(h => h.UpdatedDate)
                .FirstOrDefault();
            var now = DateTime.Now;
            var activeDiscount = productDetail.DiscountPriceDetails?
                .FirstOrDefault(x => x.DiscountPrice != null
				&& x.DiscountPrice.IsActive == true)
                ?.DiscountPrice;


            return new ProductDetailViewModel
            {
                DetailId = productDetail.DetailId,
                ColorName = productDetail.Color?.Name,
                SizeName = productDetail.Size?.Name,
                FabricName = productDetail.Fabric?.Name,
                ProductName = productDetail.Product?.ProductName,
                InputPrice = latestPrice?.InputPrice,
                SellPrice = latestPrice?.SellPrice,
                LatestPriceDate = latestPrice?.UpdatedDate ,
                DiscountPercent = activeDiscount?.DiscountPercent ?? 0,
                Images = productDetail.Images?.Select(x => x.FullPath).ToList() ?? [],
            };
        }

        public static ImageProductViewModel ToImageProductViewModel(ImagesProduct image)
		{
			return new ImageProductViewModel
			{
				FullPath = string.IsNullOrEmpty(image.FullPath)
					? $"http://localhost:5014/StaticFiles/Images/{image.Path}"
					: image.FullPath,
				ProductDetailId = image.ProductDetailId,
				Id = image.Id
			};
		}
		public static ProductResponseDTO ToCreateAndUpdateProductResponse(Product product)
		{
			return new ProductResponseDTO
			{
				ProductId = product.ProductId,
				ProductName = product.ProductName ?? string.Empty,
				Description = product.Description ?? string.Empty,
				CategoryId = product.CategoryId ?? 0,
				Status = product.Status ?? false,
			};
		}
		public static ProductDetailDTO ToCreateProductDetailResponse(ProductDetail detail)
		{
			return new ProductDetailDTO
			{
				DetailId = detail.DetailId,
				ProductId = detail.ProductId,
				SizeId = detail.SizeId,
				ColorId = detail.ColorId,
				FabricId = detail.FabricId
			};
		}

		public static CreateImageResponse ToCreateImageResponse(ImagesProduct image)
		{
			return new CreateImageResponse
			{
				ImageId = image.Id,
				ProductDetailId = image.ProductDetailId,
				ImageUrl = string.IsNullOrEmpty(image.FullPath)
					? $"http://localhost:5014/StaticFiles/Images/{image.Path}"
					: image.FullPath
			};
		}

	}
}
