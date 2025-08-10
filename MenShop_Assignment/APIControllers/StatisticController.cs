using MenShop_Assignment.Models;
using MenShop_Assignment.Models.Statistics;
using MenShop_Assignment.Repositories;
using MenShop_Assignment.Repositories.StatisticRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MenShop_Assignment.APIControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticRepository _repository;

        public StatisticController(IStatisticRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("dynamic")]
        public async Task<IActionResult> GetDynamicStatistics(
            [FromQuery] StatisticMode mode,
            [FromQuery] int? year,
            [FromQuery] int? month,
            [FromQuery] int? branchId)
        {
            try
            {
                var request = new DynamicStatisticRequest
                {
                    Mode = mode,
                    Year = year,
                    Month = month,
                    BranchId = branchId
                };

                var result = await _repository.GetDynamicStatisticsAsync(request);
                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                var badResponse = new ApiResponseModel<object>(false, ex.Message, null, 400);
                return BadRequest(badResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponseModel<object>(false, "Đã xảy ra lỗi trong quá trình xử lý.", null, 500, new List<string> { ex.Message });
                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopProducts([FromQuery] int top = 10)
        {
            var result = await _repository.GetTopBestSellingProductsAsync(top);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("top-customers")]
        public async Task<IActionResult> GetTopCustomers([FromQuery] int top = 10)
        {
            var result = await _repository.GetTopCustomersAsync(top);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }


}
