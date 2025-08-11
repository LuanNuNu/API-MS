using MenShop_Assignment.Models;
using MenShop_Assignment.Repositories.StorageRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenShop_Assignment.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
		private readonly IStorageRepository _storageRepo;

		public StorageController(IStorageRepository storageRepo)
		{
			_storageRepo = storageRepo;
		}

		[HttpGet]
        [Authorize(Roles = "Admin, RevenueManager, Factory, WarehouseManager")]
        public async Task<IActionResult> GetAll()
		{
			var storages = await _storageRepo.GetAllStoragesAsync();
			return Ok(new ApiResponseModel<List<StorageViewModel>>(true, "Lấy danh sách kho thành công", storages, 200));
		}

		[HttpGet("{storageId}/products")]
        [Authorize(Roles = "Admin, RevenueManager, Factory, WarehouseManager")]

        public async Task<IActionResult> GetProductsByStorageId(int storageId)
		{
			var products = await _storageRepo.GetProductsByStorageIdAsync(storageId);
			if (products == null)
				return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy sản phẩm trong kho", null, 404));
			return Ok(new ApiResponseModel<List<ProductViewModel>>(true, "Lấy sản phẩm trong kho thành công", products, 200));
		}

		[HttpGet("product/{productId}/details")]
        [Authorize(Roles = "Admin, RevenueManager, Factory, WarehouseManager")]

        public async Task<IActionResult> GetDetailsByProductId(int productId)
		{
			var details = await _storageRepo.GetDetailsByProductId(productId);
			if (details == null)
				return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy chi tiết sản phẩm trong kho", null, 404));
			return Ok(new ApiResponseModel<List<ProductDetailViewModel>>(true, "Lấy chi tiết sản phẩm thành công", details, 200));
		}
	}
}
