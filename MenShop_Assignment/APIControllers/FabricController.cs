using MenShop_Assignment.Repositories.FabricRepositories;
using Microsoft.AspNetCore.Mvc;
using MenShop_Assignment.Models;
using Microsoft.AspNetCore.Authorization;

namespace MenShop_Assignment.APIControllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class FabricController : ControllerBase
	{
		private readonly IFabricRepository _fabricRepo;

		public FabricController(IFabricRepository fabricRepo)
		{
			_fabricRepo = fabricRepo;
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> GetAll()
		{
			var fabrics = await _fabricRepo.GetAllFabric();
			return Ok(new ApiResponseModel<List<FabricViewModel>>(true, "Lấy danh sách chất liệu thành công", fabrics, 200));
		}

		[HttpGet("{id}")]
        [Authorize(Roles = "Admin, RevenueManager, Factory")]
        public async Task<IActionResult> GetById(int id)
		{
			var fabric = await _fabricRepo.GetByIdFabric(id);
			if (fabric == null)
				return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy chất liệu", null, 404));

			return Ok(new ApiResponseModel<FabricViewModel>(true, "Lấy chất liệu thành công", fabric, 200));
		}

		[HttpPost]
        [Authorize(Roles = "Admin, RevenueManager, Factory")]
        public async Task<IActionResult> Create([FromBody] string name)
		{
			var result = await _fabricRepo.CreateFabric(name);
			if (!result)
				return BadRequest(new ApiResponseModel<object>(false, "Không tạo được chất liệu", null, 400));

			return Ok(new ApiResponseModel<object>(true, "Tạo chất liệu thành công", null, 201));
		}

		[HttpPut("{id}")]
        [Authorize(Roles = "Admin, RevenueManager, Factory")]
        public async Task<IActionResult> Update(int id, [FromBody] string newName)
		{
			var result = await _fabricRepo.UpdateFabric(id, newName);
			if (!result)
				return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy chất liệu để cập nhật", null, 404));

			return Ok(new ApiResponseModel<object>(true, "Cập nhật chất liệu thành công", null, 200));
		}

		[HttpDelete("{id}")]
        [Authorize(Roles = "Admin, RevenueManager, Factory")]
        public async Task<IActionResult> Delete(int id)
		{
			var result = await _fabricRepo.DeleteFabric(id);
			if (!result)
				return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy chất liệu để xoá", null, 404));

			return Ok(new ApiResponseModel<object>(true, "Xoá chất liệu thành công", null, 200));
		}
	}
}
