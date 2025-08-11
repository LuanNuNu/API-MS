using MenShop_Assignment.Models;
using MenShop_Assignment.Repositories.SizeRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenShop_Assignment.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizeController : ControllerBase
    {
		private readonly ISizeRepository _sizeRepo;

		public SizeController(ISizeRepository sizeRepo)
		{
			_sizeRepo = sizeRepo;
		}

		[HttpGet]
        [Authorize(Roles = "Admin, RevenueManager, Factory, WarehouseManager")]
        public async Task<IActionResult> GetAll()
		{
			var sizes = await _sizeRepo.GetAllSize();
			return Ok(new ApiResponseModel<List<SizeViewModel>>(true, "Lấy danh sách size thành công", sizes, 200));
		}

		[HttpGet("{id}")]
        [Authorize(Roles = "Admin, RevenueManager, Factory, WarehouseManager")]
        public async Task<IActionResult> GetById(int id)
		{
			var size = await _sizeRepo.GetByIdSize(id);
			if (size == null)
				return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy size", null, 404));
			return Ok(new ApiResponseModel<SizeViewModel>(true, "Lấy size thành công", size, 200));
		}

		[HttpPost]
        [Authorize(Roles = "Admin, RevenueManager, Factory, WarehouseManager")]
        public async Task<IActionResult> Create([FromBody] string name)
		{
			var result = await _sizeRepo.CreateSize(name);
			if (!result)
				return BadRequest(new ApiResponseModel<object>(false, "Không tạo được size", null, 400));
			return Ok(new ApiResponseModel<object>(true, "Tạo size thành công", null, 201));
		}

		[HttpPut("{id}")]
        [Authorize(Roles = "Admin, RevenueManager, Factory, WarehouseManager")]
        public async Task<IActionResult> Update(int id, [FromBody] string name)
		{
			var result = await _sizeRepo.UpdateSize(id, name);
			if (!result)
				return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy size để cập nhật", null, 404));
			return Ok(new ApiResponseModel<object>(true, "Cập nhật size thành công", null, 200));
		}

		[HttpDelete("{id}")]
        [Authorize(Roles = "Admin, RevenueManager, Factory, WarehouseManager")]

        public async Task<IActionResult> Delete(int id)
		{
			var result = await _sizeRepo.DeleteSize(id);
			if (!result)
				return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy size để xoá", null, 404));
			return Ok(new ApiResponseModel<object>(true, "Xoá size thành công", null, 200));
		}
	}
}
