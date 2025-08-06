using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using MenShop_Assignment.Repositories.CustomerAddressRepositories;
using Microsoft.AspNetCore.Mvc;

namespace MenShop_Assignment.APIControllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CustomerAddressController : ControllerBase
	{
		private readonly ICustomerAddressRepository _addressRepo;

		public CustomerAddressController(ICustomerAddressRepository addressRepo)
		{
			_addressRepo = addressRepo;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var addresses = await _addressRepo.GetAllAsync();
			return Ok(new ApiResponseModel<List<CustomerAddressViewModel>>(true, "Lấy tất cả địa chỉ thành công", addresses, 200));
		}

		[HttpGet("customer/{customerId}")]
		public async Task<IActionResult> GetByCustomerId(string customerId)
		{
			var addresses = await _addressRepo.GetByCustomerIdAsync(customerId);
			return Ok(new ApiResponseModel<List<CustomerAddressViewModel>>(true, "Lấy địa chỉ theo khách hàng thành công", addresses, 200));
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateUpdateCustomerAddressDTO dto)
		{
			var success = await _addressRepo.CreateAsync(dto);
			if (!success)
				return BadRequest(new ApiResponseModel<object>(false, "Không tạo được địa chỉ", null, 400));

			return Ok(new ApiResponseModel<object>(true, "Tạo địa chỉ thành công", null, 201));
		}

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateCustomerAddressDTO dto)
        {
            var success = await _addressRepo.UpdateAsync(id, dto); 
            if (!success)
                return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy địa chỉ để cập nhật", null, 404));

            return Ok(new ApiResponseModel<object>(true, "Cập nhật địa chỉ thành công", null, 200));
        }


        [HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var success = await _addressRepo.DeleteAsync(id);
			if (!success)
				return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy địa chỉ để xoá", null, 404));

			return Ok(new ApiResponseModel<object>(true, "Xoá địa chỉ thành công", null, 200));
		}
	}
}
