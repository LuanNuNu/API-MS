using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using MenShop_Assignment.Repositories.DiscountPriceRepository;
using Microsoft.AspNetCore.Mvc;

namespace MenShop_Assignment.APIControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscountPriceController : ControllerBase
    {
        private readonly IDiscountPriceRepository _repo;

        public DiscountPriceController(IDiscountPriceRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDiscount()
        {
            var result = await _repo.GetAllDiscountPrice();
            return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, result.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest("Id không hợp lệ.");

            var result = await _repo.GetByIdDiscountPrice(id);
            return result.IsSuccess && result.Data != null
                ? Ok(result.Data)
                : StatusCode(result.StatusCode, result.Message);
        }

        [HttpGet("detail/{discountId}")]
        public async Task<IActionResult> GetAllDetailDiscount(int discountId)
        {
            if (discountId <= 0)
                return BadRequest("Id không hợp lệ.");

            var result = await _repo.GetProductDetailsByDiscountId(discountId);
            return result.IsSuccess && result.Data != null && result.Data.Any()
                ? Ok(result.Data)
                : StatusCode(result.StatusCode, result.Message);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDiscount([FromBody] CreateDiscountPriceDTO discount)
        {
            if (discount == null ||
                string.IsNullOrWhiteSpace(discount.Name) ||
                discount.StartTime >= discount.EndTime)
            {
                return BadRequest("Dữ liệu không hợp lệ: tên không được trống, thời gian không hợp lệ.");
            }

            var result = await _repo.CreateDiscount(discount);
            return result.IsSuccess
                ? StatusCode(result.StatusCode, result.Message)
                : StatusCode(result.StatusCode, result.Message);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiscount(int id, [FromBody] CreateDiscountPriceDTO discount)
        {
            if (id <= 0 || discount == null)
                return BadRequest("Dữ liệu không hợp lệ.");

            var result = await _repo.UpdateDiscount(id, discount);
            return result.IsSuccess
                ? Ok(result.Message)
                : StatusCode(result.StatusCode, result.Message);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            var result = await _repo.DeleteDiscount(id);
            return result.IsSuccess
                ? Ok(result.Message)
                : StatusCode(result.StatusCode, result.Message);
        }

        [HttpPost("detail")]
        public async Task<IActionResult> CreateDiscountDetail([FromBody] CreateDiscountDetailDTO dto)
        {
            if (dto == null || dto.discountPriceId <= 0 || dto.productDetailIds == null || !dto.productDetailIds.Any())
            {
                return BadRequest(new ApiResponseModel<object>(
                    false,
                    "Dữ liệu không hợp lệ.",
                    false,
                    400
                ));
            }

            var result = await _repo.CreateDiscountDetails(dto);

            return StatusCode(result.StatusCode, result);
        }


        [HttpPut("detail/{id}")]
        public async Task<IActionResult> UpdateDiscountDetail(int id, [FromBody] UpdateDiscountDetailDTO detail)
        {
            if (id <= 0 || detail == null)
                return BadRequest("Dữ liệu không hợp lệ.");

            var result = await _repo.UpdateDiscountDetail(id, detail);
            return result.IsSuccess
                ? Ok(result.Message)
                : StatusCode(result.StatusCode, result.Message);
        }


        [HttpDelete("detail/{id}")]
        public async Task<IActionResult> DeleteDiscountDetail(int id)
        {
            var result = await _repo.DeleteDiscountDetail(id);
            return result.IsSuccess
                ? Ok(result.Message)
                : StatusCode(result.StatusCode, result.Message);
        }


        [HttpPut("updateStatus/{discountId}")]
        public async Task<IActionResult> ToggleStatusDiscount(int discountId)
        {
            var success = await _repo.UpdateDiscountStatusAsync(discountId);
            return success
                ? Ok("Cập nhật trạng thái chương trình thành công!")
                : NotFound("Không tìm thấy chương trình để cập nhật.");
        }




        [HttpGet("detail/product/{productDetailId}")]
        public async Task<IActionResult> GetDiscountDetailByProductDetailId(int productDetailId)
        {
            if (productDetailId <= 0)
                return BadRequest("ProductDetailId không hợp lệ.");

            var result = await _repo.GetDiscountDetailsByProductDetailId(productDetailId);
            return StatusCode(result.StatusCode, result); 
        }


        [HttpGet("detail/item/{detailId}")]
        public async Task<IActionResult> GetDiscountDetailById(int detailId)
        {
            if (detailId <= 0)
                return BadRequest("Id không hợp lệ.");

            var result = await _repo.GetByIdDiscountDetailPrice(detailId);
            return result.IsSuccess && result.Data != null
                ? Ok(result.Data)
                : StatusCode(result.StatusCode, result.Message);
        }

    }
}
