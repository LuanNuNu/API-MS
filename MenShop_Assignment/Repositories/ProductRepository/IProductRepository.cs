using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using MenShop_Assignment.Models.ProductModels.ReponseDTO;

namespace MenShop_Assignment.Repositories.Product
{
    public interface IProductRepository
    {
		Task<ProductViewModel?> GetProductByIdAsync(int productId);
		Task<List<ProductViewModel>> GetProductsByCategoryIdAsync(int categoryId);
        Task<List<ProductDetailViewModel>> GetProductDetailsByProductIdAsync(int productId);
		Task<List<ImageProductViewModel>> GetImgByProductDetailIdAsync(int productDetailId);
		Task<IEnumerable<ProductViewModel>> GetAllProductsAsync();
		Task<ProductResponseDTO> CreateProductOnlyAsync(CreateProductDTO dto);
		Task<CreateProductDetailResponse> AddProductDetailsAsync(AddProductDetailDTO dto);
		Task<List<CreateImageResponse>> AddImagesToDetailAsync(int detailId, List<string> imageUrls);
		Task<ProductResponseDTO> UpdateProductAsync(int productId, UpdateProductDTO dto);
		Task<ProductDetailResponse> UpdateProductDetailsAsync(UpdateProductDetailDTO details);
		Task<ImageResponse> UpdateProductDetailImagesAsync(int detailId, List<UpdateImageDTO> images);
		Task<bool> UpdateProductStatusAsync(int productId);
		Task DeleteProductAsync(int productId);
		Task DeleteProductDetailAsync(int detailId);
		Task DeleteImageAsync(int imageId);
	}
}
