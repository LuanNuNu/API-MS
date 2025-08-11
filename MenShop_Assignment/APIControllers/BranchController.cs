using MenShop_Assignment.DTOs;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;
using MenShop_Assignment.Repositories.BranchesRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MenShop_Assignment.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
		private readonly IBranchRepository _branchRepository;
		public BranchController(IBranchRepository branchRepository)
		{
			_branchRepository = branchRepository;
		}
		[HttpGet("getbranches")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllBranches()
		{
			var branches = await _branchRepository.GetBranchesAsync();
			if (branches == null)
				return NotFound(new ApiResponseModel<List<BranchViewModel>>(false, "Không tìm thấy chi nhánh", null, 404));
			return Ok(new ApiResponseModel<List<BranchViewModel>>(true, "Lấy danh sách chi nhánh thành công", branches, 200));
		}

		[HttpGet("getbranch/{id}")]
        [AllowAnonymous]

        public async Task<IActionResult> GetBranchById(int id)
		{
			var branch = await _branchRepository.GetBranchByIdAsync(id);
			if (branch == null)
				return NotFound(new ApiResponseModel<BranchViewModel>(false, "Chi nhánh không tồn tại", null, 404));
			return Ok(new ApiResponseModel<BranchViewModel>(true, "Lấy chi nhánh thành công", branch, 200));
		}

		[HttpPost("create")]
        [Authorize(Roles = "Admin, RevenueManager")]
        public async Task<IActionResult> CreateBranch([FromBody] CreateUpdateBranchDTO dto)
		{
			var branch = await _branchRepository.CreateBranchAsync(dto);
			if (branch == null)
				return BadRequest(new ApiResponseModel<object>(false, "Tạo chi nhánh thất bại", null, 400));
			var viewModel = BranchMapper.ToBranchViewModel(branch);
			return Ok(new ApiResponseModel<BranchViewModel>(true, "Tạo chi nhánh thành công", viewModel, 201));
		}

        [HttpPut("{branchId}")]
        [Authorize(Roles = "Admin, RevenueManager")]
        public async Task<IActionResult> UpdateBranch(int branchId, [FromBody] CreateUpdateBranchDTO dto)
        {
            var branch = await _branchRepository.UpdateBranchAsync(branchId,dto);
			if (branch == null)
				return NotFound(new ApiResponseModel<object>(false, "Không cập nhật được chi nhánh", null, 404));
			var viewModel = BranchMapper.ToBranchViewModel(branch);
			return Ok(new ApiResponseModel<BranchViewModel>(true, "Cập nhập chi nhánh thành công", viewModel, 201));
		}
        [HttpGet("products")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBranchProducts([FromQuery] int? branchId, [FromQuery] int? categoryId)
        {
            var role = User?.FindFirst(ClaimTypes.Role)?.Value ?? "Customer";

            var products = await _branchRepository.GetBranchProductsAsync(branchId, categoryId, role);

            if (products == null || products.Count == 0)
                return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy sản phẩm", null, 404));

            return Ok(new ApiResponseModel<List<ProductViewModel>>(true, "Lấy sản phẩm thành công", products, 200));
        }


        [HttpGet("products/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductDetailsInBranch(int productId, [FromQuery] int? branchId)
        {
            if (productId <= 0)
                return BadRequest(new ApiResponseModel<object>(false, "Mã sản phẩm không hợp lệ", null, 400));

            var role = User?.FindFirst(ClaimTypes.Role)?.Value ?? "Customer";

            var details = await _branchRepository.GetDetailProductBranchAsync(branchId, productId, role);

            if (details == null || details.Count == 0)
                return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy chi tiết sản phẩm", null, 404));

            return Ok(new ApiResponseModel<object>(true, "Lấy chi tiết sản phẩm thành công", details, 200));
        }

        [HttpGet("productDetails/{productDetailId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductDetail([FromQuery] int branchId, int productDetailId)
        {
            if (productDetailId <= 0)
                return BadRequest(new ApiResponseModel<object>(false, "Mã chi tiết sản phẩm không hợp lệ.", null, 400));

            var role = User?.FindFirst(ClaimTypes.Role)?.Value ?? "Customer";

            // Sửa thứ tự tham số cho đúng
            var detail = await _branchRepository.GetProductDetailByIdAsync(productDetailId, branchId, role);

            if (detail == null)
                return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy chi tiết sản phẩm.", null, 404));
            return Ok(new ApiResponseModel<object>(true, "Lấy chi tiết sản phẩm thành công", detail, 200));
        }

        [HttpGet("{branchId}/search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchProductInBranch(int branchId, [FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest(new ApiResponseModel<object>(false, "Chuỗi tìm kiếm không hợp lệ", null, 400));


            bool isNumber = int.TryParse(searchTerm, out int idSearch);
            var results = await _branchRepository.SmartSearchProductsAsync(branchId, searchTerm, isNumber ? idSearch : (int?)null);

            return Ok(new ApiResponseModel<object>(true, "Tìm kiếm thành công", results, 200));
        }


        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin, RevenueManager")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            try
            {
                var success = await _branchRepository.DeleteBranchAsync(id);
                if (!success)
                    return NotFound(new ApiResponseModel<object>(false, "Chi nhánh không tồn tại", null, 404));

                return Ok(new ApiResponseModel<object>(true, "Xoá chi nhánh thành công", null, 200));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponseModel<object>(false, ex.Message, null, 400));
            }
        }

    }
}
