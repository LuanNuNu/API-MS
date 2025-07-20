using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using MenShop_Assignment.Repositories.CartRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenShop_Assignment.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    /*[Authorize]*/
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

		[HttpGet("getcart/{customerId}")]
		public async Task<IActionResult> GetCart(string customerId)
		{
			var cart = await _cartRepository.GetCartByCustomerIdAsync(customerId);
			if (cart == null)
				return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy giỏ hàng", null, 404));
			return Ok(new ApiResponseModel<CartViewModel>(true, "Lấy giỏ hàng thành công", cart, 200));
		}


		[HttpPost("add")]
		public async Task<IActionResult> AddToCart([FromBody] CartActionDTO dto)
		{
			if (dto == null || dto.ProductDetailId == 0 || dto.Quantity <= 0 || string.IsNullOrEmpty(dto.CustomerId))
				return BadRequest(new ApiResponseModel<object>(false, "Dữ liệu không hợp lệ", null, 400));

			if(dto.CustomerId==null)
				return NotFound(new ApiResponseModel<object>(false,"Không tìm thấy người dùng", null, 404));

			var result = await _cartRepository.AddToCartAsync(dto.CustomerId, dto.ProductDetailId, dto.Quantity);
			if (!result)
				return NotFound(new ApiResponseModel<object>(false, "Không thêm được vào giỏ hàng", null, 404));

			return Ok(new ApiResponseModel<object>(true, "Thêm vào giỏ hàng thành công", null, 200));
		}

		[HttpPut("update")]
		public async Task<IActionResult> UpdateQuantity([FromBody] CartActionDTO dto)
		{
			if (dto.Quantity < 1)
				return BadRequest(new ApiResponseModel<object>(false, "Số lượng phải lớn hơn 0", null, 400));

			if (dto.CustomerId == null)
				return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy người dùng", null, 404));

			var result = await _cartRepository.UpdateQuantityAsync(dto.CustomerId, dto.ProductDetailId, dto.Quantity);
			if (!result)
				return NotFound(new ApiResponseModel<object>(false, "Không cập nhật được số lượng sản phẩm", null, 404));

			return Ok(new ApiResponseModel<object>(true, "Cập nhật số lượng thành công", null, 200));
		}

		[HttpDelete("remove")]
		public async Task<IActionResult> RemoveFromCart([FromBody] CartActionDTO dto)
		{
			var result = await _cartRepository.RemoveFromCartAsync(dto.CustomerId, dto.ProductDetailId);
			if (!result)
				return NotFound(new ApiResponseModel<object>(false, "Không xoá được sản phẩm khỏi giỏ hàng", null, 404));

			return Ok(new ApiResponseModel<object>(true, "Đã xoá sản phẩm khỏi giỏ hàng", null, 200));
		}
	}
}
