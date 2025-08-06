using MenShop_Assignment.Datas;
using MenShop_Assignment.Repositories.DiscountPriceRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MenShop_Assignment.DTOs;


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
            return result.IsSuccess? Ok(result.Data) : StatusCode(500, result.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("Id không hợp lệ.");

            var result = await _repo.GetByIdDiscountPrice(id);
            if (!result.IsSuccess || result.Data == null)
                return NotFound(result.Message ?? $"Không tìm thấy khuyến mãi với ID = {id}");

            return Ok(result.Data);
        }

        [HttpGet("detail/{discountId}")]
        public async Task<IActionResult> GetAllDetailDiscount(int discountId)
        {
            if (discountId <= 0) return BadRequest("Id không hợp lệ.");

            var result = await _repo.GetProductDetailsByDiscountId(discountId);
            if (!result.IsSuccess || result.Data == null || !result.Data.Any())
                return NotFound($"Không có sản phẩm nào áp dụng khuyến mãi với ID = {discountId}");

            return Ok(result.Data);
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

            var discountPrice = new DiscountPrice
            {
                Name = discount.Name,
                DiscountPercent = discount.DiscountPercent,
                StartTime = discount.StartTime,
                EndTime = discount.EndTime,
            };

            var result = await _repo.CreateDiscount(discountPrice);
            return result.IsSuccess ? Ok(discountPrice) : BadRequest(result.Message ?? "Thêm khuyến mãi thất bại.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiscount(int id, [FromBody] CreateDiscountPriceDTO discount)
        {
            if (id <= 0 || discount == null)
                return BadRequest("Dữ liệu không hợp lệ.");

            var check = await _repo.GetByIdDiscountPrice(id);
            if (!check.IsSuccess || check.Data == null)
                return NotFound($"Không tìm thấy khuyến mãi với ID = {id}");

            var updated = new DiscountPrice
            {
                Id = id,
                Name = discount.Name,
                DiscountPercent = discount.DiscountPercent,
                StartTime = discount.StartTime,
                EndTime = discount.EndTime,
            };

            var result = await _repo.UpdateDiscount(updated);
            return result.IsSuccess ? Ok("Cập nhật thành công") : BadRequest(result.Message ?? "Cập nhật thất bại.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            var result = await _repo.DeleteDiscount(id);
            return result.IsSuccess ? Ok("Xóa khuyến mãi thành công.") : BadRequest(result.Message ?? "Không thể xóa khuyến mãi.");
        }

        [HttpPost("detail")]
        public async Task<IActionResult> CreateDiscountDetail([FromBody] CreateDiscountDetailDTO dto)
        {
            if (dto == null || dto.discountPriceId <= 0 || dto.productDetailIds == null || !dto.productDetailIds.Any())
                return BadRequest("Dữ liệu không hợp lệ.");

            var failedIds = new List<int>();

            foreach (var productDetailId in dto.productDetailIds)
            {
                var check = await _repo.GetDiscountDetailsByProductDetailId(productDetailId);
                if (check.IsSuccess && check.Data.Any())
                {
                    failedIds.Add(productDetailId);
                    continue;
                }

                var detail = new DiscountPriceDetail
                {
                    discountPriceId = dto.discountPriceId,
                    productDetailId = productDetailId
                };

                var result = await _repo.CreateDiscountDetail(detail);
                if (!result.IsSuccess)
                    failedIds.Add(productDetailId);
            }

            if (failedIds.Any())
            {
                return StatusCode(207, new
                {
                    Message = "Một số sản phẩm đã có chương trình khuyến mãi hoặc lỗi khi thêm.",
                    FailedProductDetailIds = failedIds
                });
            }

            return Ok("Tất cả sản phẩm đã được thêm vào khuyến mãi.");
        }

        [HttpPut("detail/{id}")]
        public async Task<IActionResult> UpdateDiscountDetail(int id, [FromBody] UpdateDiscountDetailDTO detail)
        {
            if (id <= 0 || detail == null)
                return BadRequest("Dữ liệu không hợp lệ.");

            var existing = await _repo.GetByIdDiscountDetailPrice(id);
            if (!existing.IsSuccess || existing.Data == null)
                return NotFound($"Không tìm thấy chi tiết với ID = {id}");

            var update = new DiscountPriceDetail
            {
                Id = id,
                productDetailId = detail.productDetailId,
                discountPriceId = detail.discountPriceId
            };

            var result = await _repo.UpdateDiscountDetail(update);
            return result.IsSuccess ? Ok("Cập nhật chi tiết thành công.") : BadRequest(result.Message ?? "Cập nhật thất bại.");
        }

        [HttpDelete("detail/{id}")]
        public async Task<IActionResult> DeleteDiscountDetail(int id)
        {
            var result = await _repo.DeleteDiscountDetail(id);
            return result.IsSuccess ? Ok("Xóa chi tiết thành công.") : NotFound(result.Message ?? "Không thể xóa chi tiết.");
        }

        [HttpGet("detail/product/{productDetailId}")]
        public async Task<IActionResult> GetDiscountDetailByProductDetailId(int productDetailId)
        {
            if (productDetailId <= 0) return BadRequest("ProductDetailId không hợp lệ.");

            var result = await _repo.GetDiscountDetailsByProductDetailId(productDetailId);
            if (!result.IsSuccess || result.Data == null || !result.Data.Any())
                return NotFound($"Không tìm thấy giảm giá nào cho ProductDetailId = {productDetailId}");

            return Ok(result.Data);
        }
    }
}
