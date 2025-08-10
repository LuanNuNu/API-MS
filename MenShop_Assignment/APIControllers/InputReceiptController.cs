using Azure.Core;
using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using MenShop_Assignment.Repositories.InputReceiptRepositories;
using MenShop_Assignment.Repositories.InputReceiptRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MenShop_Assignment.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InputReceiptController : ControllerBase
    {
        private readonly IInputReceiptRepository _receiptRepository;
        public InputReceiptController(IInputReceiptRepository receiptRepository)
        {
            _receiptRepository = receiptRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var receipts = await _receiptRepository.GetInputReceipts();
            return Ok(new ApiResponseModel<List<InputReceiptViewModel>>(true, "Lấy danh sách phiếu nhập thành công", receipts, 200));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var receiptDetails = await _receiptRepository.GetByIdAsync(id);
            if (receiptDetails == null)
                return NotFound(new ApiResponseModel<object>(false, "Không tìm thấy chi tiết phiếu nhập", null, 404));

            return Ok(new ApiResponseModel<List<ProductDetailViewModel>>(true, "Lấy chi tiết phiếu nhập thành công", receiptDetails, 200));
        }

        [HttpPut("confirm/{id}")]
        public async Task<IActionResult> Confirm(int id)
        {
            var result = await _receiptRepository.ConfirmReceipt(id);
            if (!result)
                return BadRequest(new ApiResponseModel<object>(false, "Xác nhận phiếu nhập thất bại", null, 400));
            return Ok(new ApiResponseModel<object>(true, "Xác nhận phiếu nhập thành công", null, 200));
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateReceipt(
            [FromBody] List<CreateReceiptDetailDTO> detailDTOs,
            [FromQuery] string ManagerId)
        {
            if (detailDTOs == null || !detailDTOs.Any())
            {
                return BadRequest(new ApiResponseModel<object>(
                    false,
                    "Danh sách chi tiết phiếu nhập không được để trống",
                    null,
                    400));
            }

            if (string.IsNullOrEmpty(ManagerId))
            {
                return BadRequest(new ApiResponseModel<object>(
                    false,
                    "ManagerId không được để trống",
                    null,
                    400));
            }

            var result = await _receiptRepository.CreateInputReceipt(detailDTOs, ManagerId);

            if (!result)
            {
                return BadRequest(new ApiResponseModel<object>(
                    false,
                    "Tạo phiếu nhập thất bại",
                    null,
                    400));
            }

            return Ok(new ApiResponseModel<object>(
                true,
                "Tạo phiếu nhập thành công",
                null,
                201));
        }

        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _receiptRepository.CancelReceipt(id);
            if (!result)
                return BadRequest(new ApiResponseModel<object>(false, "Huỷ phiếu nhập thất bại", null, 400));
            return Ok(new ApiResponseModel<object>(true, "Huỷ phiếu nhập thành công", null, 200));
        }
    }
}