using MenShop_Assignment.DTOs;

namespace MenShop_Assignment.Models.ProductModels.ReponseDTO
{
	public class CreateProductDetailResponse
	{
		public List<ProductDetailResult> Results { get; set; } = new();
	}

	public class ProductDetailResult
	{
		public bool IsSuccess { get; set; }
		public string Message { get; set; } = string.Empty;
		public ProductDetailDTO Detail { get; set; } = new();
	}
}
