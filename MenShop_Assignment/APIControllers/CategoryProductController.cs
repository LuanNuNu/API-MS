using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using MenShop_Assignment.Repositories.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenShop_Assignment.APIControllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CategoryProductController : ControllerBase
	{
		private readonly ICategoryProductRepository _categoryRepo;

		public CategoryProductController(ICategoryProductRepository categoryRepo)
		{
			_categoryRepo = categoryRepo;
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> GetAll()
		{
			var categories = await _categoryRepo.GetAllCategoriesAsync();
			if (categories == null || !categories.Any())
				return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy danh mục sản phẩm nào", null, 404));

			return Ok(new ApiResponseModel<List<CategoryProductViewModel>>(true, "Lấy danh mục sản phẩm thành công", categories, 200));
		}

		[HttpGet("{id}")]
        [Authorize(Roles = "Admin, RevenueManager")]
        public async Task<IActionResult> GetById(int id)
		{
			var category = await _categoryRepo.GetCategoryByIdAsync(id);
			if (category == null)
				return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy danh mục sản phẩm", null, 404));

			return Ok(new ApiResponseModel<CategoryProductViewModel>(true, "Lấy danh mục sản phẩm thành công", category, 200));
		}

		[HttpPost]
        [Authorize(Roles = "Admin, RevenueManager")]

        public async Task<IActionResult> Create([FromBody] CreateUpdateCategoryDTO dto)
		{
			var success = await _categoryRepo.CreateCategoryAsync(dto);
			if (!success)
				return BadRequest(new ApiResponseModel<object>(false, "Tạo danh mục sản phẩm thất bại", null, 400));

			return Ok(new ApiResponseModel<object>(true, "Tạo danh mục sản phẩm thành công", null, 201));
		}

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, RevenueManager")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateCategoryDTO dto)
        {
            var currentCategory = await _categoryRepo.GetCategoryByIdAsync(id);
            if (currentCategory == null)
            {
                return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy danh mục để cập nhật", null, 404));
            }
            dto.Id = id;
            var success = await _categoryRepo.UpdateCategoryAsync(dto);
            if (!success)
            {
                return StatusCode(500, new ApiResponseModel<object>(false, "Đã xảy ra lỗi khi cập nhật danh mục sản phẩm", null, 500));
            }

            return Ok(new ApiResponseModel<object>(true, "Cập nhật danh mục sản phẩm thành công", null, 200));
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, RevenueManager")]
        public async Task<IActionResult> Delete(int id)
		{
			var success = await _categoryRepo.RemoveCategoryAsync(id);
			if (!success)
				return NotFound(new ApiResponseModel<object>(false, "Không thể xóa danh mục sản phẩm", null, 404));

			return Ok(new ApiResponseModel<object>(true, "Xoá danh mục sản phẩm thành công", null, 200));
		}
	}
}
