using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using MenShop_Assignment.Repositories.OutputReceiptRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MenShop_Assignment.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OutputReceiptController : ControllerBase
    {
        private readonly IOutputReceiptRepository _outputReceiptRepository;

        public OutputReceiptController(IOutputReceiptRepository outputReceiptRepository)
        {
            _outputReceiptRepository = outputReceiptRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, RevenueManager, Factory, WarehouseManager")]
        public async Task<IActionResult> GetAll()
        {
            var receipts = await _outputReceiptRepository.GetOutputReceiptViews();
            return Ok(new ApiResponseModel<List<OutputReceiptViewModel>>(true, "Lấy danh sách phiếu xuất thành công", receipts, 200));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, RevenueManager, Factory, WarehouseManager")]
        public async Task<IActionResult> GetById(int id)
        {
            var receipt = await _outputReceiptRepository.GetById(id);
            if (receipt == null)
                return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy phiếu xuất", null, 404));
            return Ok(new ApiResponseModel<List<ProductDetailViewModel>>(true, "Lấy phiếu xuất thành công", receipt, 200));
        }

        [HttpGet("branch/{branchId}")]
        [Authorize(Roles = "Admin, RevenueManager, Factory, WarehouseManager, BranchManager")]
        public async Task<IActionResult> GetByBranch(int branchId)
        {
            var receipts = await _outputReceiptRepository.GetByBranchId(branchId);
            return Ok(new ApiResponseModel<List<OutputReceiptViewModel>>(true, "Lấy phiếu xuất theo chi nhánh thành công", receipts, 200));
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin, RevenueManager, Factory, WarehouseManager")]
        public async Task<IActionResult> Create(
            [FromBody] List<CreateReceiptDetailDTO> detailDTOs,
            [FromQuery] int branchId,
            [FromQuery] string managerId)
        {
            var result = await _outputReceiptRepository.CreateReceipt(branchId, managerId, detailDTOs);
            if (!result)
                return BadRequest(new ApiResponseModel<object>(false, "Tạo phiếu xuất thất bại", null, 400));

            return Ok(new ApiResponseModel<object>(true, "Tạo phiếu xuất thành công", null, 201));
        }


        [HttpPut("confirm/{id}")]
        [Authorize(Roles = "Admin, RevenueManager")]
        public async Task<IActionResult> Confirm(int id)
        {
            var result = await _outputReceiptRepository.ConfirmReceipt(id);
            if (!result)
                return BadRequest(new ApiResponseModel<object>(false, "Xác nhận phiếu xuất thất bại", null, 400));
            return Ok(new ApiResponseModel<object>(true, "Xác nhận phiếu xuất thành công", null, 200));
        }

        [HttpPut("cancel/{id}")]
        [Authorize(Roles = "Admin, RevenueManager")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _outputReceiptRepository.CancelReceipt(id);
            if (!result)
                return BadRequest(new ApiResponseModel<object>(false, "Huỷ phiếu xuất thất bại", null, 400));
            return Ok(new ApiResponseModel<object>(true, "Huỷ phiếu xuất thành công", null, 200));
        }
    }
}