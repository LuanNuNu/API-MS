using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using MenShop_Assignment.Repositories.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenShop_Assignment.APIControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
		private readonly IProductRepository _productRepository;

		public ProductController(IProductRepository productRepository)
		{
			_productRepository = productRepository;
		}

		// GET: api/Product
		[HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetAllProducts()
		{
			var products = await _productRepository.GetAllProductsAsync();
			return Ok(products);
		}
		[HttpGet("productDetails/{productId}")]
		public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetProductDetails(int productId)
		{
			var productDetails = await _productRepository.GetProductDetailsByProductIdAsync(productId);
			return Ok(productDetails);
		}

		//up
		[HttpGet("productDetails/images/{productDetailId}")]
		public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetImgProductDetails(int productDetailId)
		{
			var productImgDetails = await _productRepository.GetImgByProductDetailIdAsync(productDetailId);
			return Ok(productImgDetails);
		}
		[HttpGet("{id}")]
		public async Task<ActionResult<ProductViewModel>> GetProductById(int id)
		{
			var product = await _productRepository.GetProductByIdAsync(id);
			if (product == null)
			{
				return NotFound(new { message = $"Không tìm thấy sản phẩm với ID = {id}" });
			}
			return Ok(product);
		}
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<List<ProductViewModel>>> GetProductsByCategoryId(int categoryId)
        {
            var products = await _productRepository.GetProductsByCategoryIdAsync(categoryId);
            if (products == null || !products.Any())
            {
                return NotFound(new { message = $"Không tìm thấy sản phẩm với Category ID = {categoryId}" });
            }
            return Ok(products);
        }



        [HttpPut("product/{id}")]
		public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDTO dto)
		{
			var result = await _productRepository.UpdateProductAsync(id, dto);
			if (!result.IsSuccess)
				return BadRequest(result);

			return Ok(result);
		}
		[HttpPut("product-detail")]
		public async Task<IActionResult> UpdateProductDetail([FromBody] UpdateProductDetailDTO detailDto)
		{
			var result = await _productRepository.UpdateProductDetailsAsync(detailDto);
			if (!result.IsSuccess)
				return BadRequest(result);

			return Ok(result);
		}
		[HttpPut("product-detail/{detailId}/images")]
		public async Task<IActionResult> UpdateProductDetailImages(int detailId, [FromBody] List<UpdateImageDTO> images)
		{
			var result = await _productRepository.UpdateProductDetailImagesAsync(detailId, images);
			if (!result.IsSuccess)
				return BadRequest(result);

			return Ok(result);
		}



		[HttpPut("updateStatus/{productId}")]
		public async Task<IActionResult> ToggleStatusProduct(int productId)
		{
			var result = await _productRepository.UpdateProductStatusAsync(productId);
			if (result)
			{
				return Ok("Cập nhật trạng thái sản phẩm thành công!");
			}

			return NotFound("Không tìm thấy sản phẩm để cập nhật.");
		}

		[HttpDelete("details/{detailId}")]
		public async Task<IActionResult> DeleteProductDetail(int detailId)
		{
			try
			{
				await _productRepository.DeleteProductDetailAsync(detailId);
				return Ok("Xóa chi tiết sản phẩm thành công!");
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpDelete("{productId}")]
		public async Task<IActionResult> DeleteProduct(int productId)
		{
			try
			{
				await _productRepository.DeleteProductAsync(productId);
				return Ok("Xóa sản phẩm thành công!");
			}
			catch (Exception ex)
			{
				return BadRequest(new
				{
					message = ex.Message,
					details = ex.StackTrace
				});
			}
		}
		//up1
		[HttpDelete("details/images/{imgId}")]
		public async Task<IActionResult> DeleteImgProductDetail(int imgId)
		{
			try
			{
				await _productRepository.DeleteImageAsync(imgId);
				return Ok("Xóa ảnh chi tiết sản phẩm thành công!");
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpPost("create")]
		public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO dto)
		{
			var result = await _productRepository.CreateProductOnlyAsync(dto);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result);
		}

		//up2
		[HttpPost("add-detail")]
		public async Task<IActionResult> AddProductDetail([FromBody] AddProductDetailDTO dto)
		{
			var result = await _productRepository.AddProductDetailsAsync(dto);
			if (!result.Results.Any(r => r.IsSuccess))
			{
				return BadRequest(result);
			}

			return Ok(result);
		}



		//up
		[HttpPost("add-images/{detailId}")]
		public async Task<IActionResult> AddImages(int detailId, [FromBody] List<string> imageUrls)
		{
			var result = await _productRepository.AddImagesToDetailAsync(detailId, imageUrls);
			if (result.Count == 0 || result.Any(r => !r.IsSuccess))
				return BadRequest(result);

			return Ok(result);
		}

	}
}
