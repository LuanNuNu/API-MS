using MenShop_Assignment.Models;
using MenShop_Assignment.Models.Statistics;
using MenShop_Assignment.Repositories;
using MenShop_Assignment.Repositories.StatisticRepository;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin, RevenueManager, Factory, WarehouseManager, BranchManager")]

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
        [Authorize(Roles = "Admin, RevenueManager, Factory, WarehouseManager, BranchManager")]

        public async Task<IActionResult> GetTopProducts([FromQuery] int top = 10)
        {
            var result = await _repository.GetTopBestSellingProductsAsync(top);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("top-customers")]
        [Authorize(Roles = "Admin, RevenueManager, WarehouseManager, BranchManager")]

        public async Task<IActionResult> GetTopCustomers([FromQuery] int top = 10)
        {
            var result = await _repository.GetTopCustomersAsync(top);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("top-best-selling-products-by-day")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTopBestSellingProductsByDay(
      [FromQuery] DateTime? date = null,
      [FromQuery] int top = 10,
      [FromQuery] int? branchId = null)
        {
            var result = await _repository.GetTopBestSellingProductsByDayAsync(date, top, branchId);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("total-orders-by-day")]
        [Authorize(Roles = "Admin, RevenueManager, WarehouseManager, BranchManager")]
        public async Task<IActionResult> GetTotalOrdersByDay(
            [FromQuery] DateTime? date = null,
            [FromQuery] int? branchId = null)
        {
            var result = await _repository.GetTotalOrdersByDayAsync(date, branchId);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

    }


}
