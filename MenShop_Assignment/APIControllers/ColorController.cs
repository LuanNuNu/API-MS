using MenShop_Assignment.Repositories.ColorRepository;
using Microsoft.AspNetCore.Mvc;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.APIControllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ColorController : ControllerBase
	{
		private readonly IColorRepository _colorRepo;

		public ColorController(IColorRepository colorRepo)
		{
			_colorRepo = colorRepo;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var colors = await _colorRepo.GetAllColor();
			return Ok(new ApiResponseModel<List<ColorViewModel>>(true, "Lấy danh sách màu thành công", colors, 200));
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var color = await _colorRepo.GetByIdColor(id);
			if (color == null)
				return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy màu", null, 404));

			return Ok(new ApiResponseModel<ColorViewModel>(true, "Lấy màu thành công", color, 200));
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] string name)
		{
			var result = await _colorRepo.CreateColor(name);
			if (!result)
				return BadRequest(new ApiResponseModel<object>(false, "Lỗi không thể tạo được màu", null, 400));

			return Ok(new ApiResponseModel<object>(true, "Tạo màu thành công", null, 201));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] string newName)
		{
			var result = await _colorRepo.UpdateColor(id, newName);
			if (!result)
				return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy màu để cập nhật", null, 404));

			return Ok(new ApiResponseModel<object>(true, "Cập nhật màu thành công", null, 200));
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var result = await _colorRepo.DeleteColor(id);
			if (!result)
				return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy màu để xoá", null, 404));

			return Ok(new ApiResponseModel<object>(true, "Xoá màu thành công", null, 200));
		}
	}
}
