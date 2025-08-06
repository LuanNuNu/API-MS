using MenShop_Assignment.Datas;
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

        [HttpGet("getcart")]
        public async Task<IActionResult> GetCart([FromQuery] string? customerId, [FromQuery] string? anonymousId)
        {
            if (string.IsNullOrEmpty(customerId) && string.IsNullOrEmpty(anonymousId))
            {
                return BadRequest(new ApiResponseModel<object>(false, "Thiếu thông tin người dùng", null, 400));
            }

            CartViewModel? cart = null;

            if (!string.IsNullOrEmpty(customerId))
            {
                cart = await _cartRepository.GetCartByCustomerIdAsync(customerId);
            }
            else if (!string.IsNullOrEmpty(anonymousId))
            {
                cart = await _cartRepository.GetCartByAnonymousIdAsync(anonymousId);
            }

            if (cart == null)
            {
                return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy giỏ hàng", null, 404));
            }

            return Ok(new ApiResponseModel<CartViewModel>(true, "Lấy giỏ hàng thành công", cart, 200));
        }




        [HttpPost("add")]
		public async Task<IActionResult> AddToCart([FromBody] CartActionDTO dto)
		{
			if (dto == null || dto.ProductDetailId == 0 || dto.Quantity <= 0 || string.IsNullOrEmpty(dto.CustomerId) && string.IsNullOrEmpty(dto.AnonymousId))
				return BadRequest(new ApiResponseModel<object>(false, "Dữ liệu không hợp lệ", null, 400));

			//if(dto.CustomerId==null)
			//	return NotFound(new ApiResponseModel<object>(false,"Không tìm thấy người dùng", null, 404));

			var result = await _cartRepository.AddToCartAsync(dto.CustomerId, dto.AnonymousId, dto.ProductDetailId, dto.Quantity);
			if (!result)
				return NotFound(new ApiResponseModel<object>(false, "Không thêm được vào giỏ hàng", null, 404));

			return Ok(new ApiResponseModel<object>(true, "Thêm vào giỏ hàng thành công", null, 200));
		}

		[HttpPut("update")]
		public async Task<IActionResult> UpdateQuantity([FromBody] CartActionDTO dto)
		{
			if (dto.Quantity < 1)
				return BadRequest(new ApiResponseModel<object>(false, "Số lượng phải lớn hơn 0", null, 400));

			//if (dto.CustomerId == null || dto.AnonymousId == null)
			//	return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy người dùng", null, 404));

			var reponse = await _cartRepository.UpdateQuantityAsync(dto.CustomerId, dto.AnonymousId,dto.ProductDetailId, dto.Quantity);
			if (!reponse)
				return NotFound(new ApiResponseModel<object>(false, "Không cập nhật được số lượng sản phẩm", null, 404));

			return Ok(new ApiResponseModel<object>(true, "Cập nhật số lượng thành công", null, 200));
		}

		[HttpDelete("remove")]
		public async Task<IActionResult> RemoveFromCart([FromBody] CartActionDTO dto)
		{
			var reponse = await _cartRepository.RemoveFromCartAsync(dto.CustomerId, dto.AnonymousId,dto.ProductDetailId);
			if (!reponse)
				return NotFound(new ApiResponseModel<object>(false, "Không xoá được sản phẩm khỏi giỏ hàng", null, 404));

			return Ok(new ApiResponseModel<object>(true, "Đã xoá sản phẩm khỏi giỏ hàng", null, 200));
		}

        [HttpPost("merge-cart")]
        public async Task<IActionResult> MergeCart([FromBody] MergeCartRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.CustomerId) || string.IsNullOrEmpty(request.AnonymousId))
                return BadRequest("Thiếu CustomerId hoặc AnonymousId");

            var anonymousCart = await _cartRepository.GetCartByAnonymousIdAsync(request.AnonymousId);
            if (anonymousCart == null || anonymousCart.Details.Count == 0)
                return Ok("Không có gì để merge");

            var customerCart = await _cartRepository.GetCartByCustomerIdAsync(request.CustomerId);

            if (customerCart == null)
            {

                await _cartRepository.AssignCartToCustomerAsync(anonymousCart.CartId, request.CustomerId);
            }
            else
            {

                foreach (var item in anonymousCart.Details)
                {
                    await _cartRepository.MergeItemAsync(customerCart.CartId, item);
                }

            }
            await _cartRepository.DeleteCartByAnonymousIdAsync(request.AnonymousId);
            return Ok(new ApiResponseModel<object>(true, "Merge cart thành công", null, 200));
        }

    }
}
