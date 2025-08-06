using MenShop_Assignment.DTOs;
using MenShop_Assignment.Extensions;
using MenShop_Assignment.Models.VNPay;
using MenShop_Assignment.Repositories.OrderRepository;
using MenShop_Assignment.Repositories.Product;
using MenShop_Assignment.Services.Momo;
using MenShop_Assignment.Services.PaymentServices;
using MenShop_Assignment.Services.VNPay;
using Microsoft.AspNetCore.Mvc;

namespace MenShop_Assignment.APIControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IMomoServices _momoServices;
        private readonly IOrderRepository _orderRepository;
        private readonly IVNPayService _vnPayService;
        public PaymentController(IMomoServices momoServices, IOrderRepository orderRepository, IVNPayService VnPayService, IPaymentService paymentService)
        {
            _paymentService = paymentService;
            _momoServices = momoServices;
            _orderRepository = orderRepository;
            _vnPayService = VnPayService;

        }
        [HttpPost("create-MomoPayment")]
        public async Task<IActionResult> CreatePayment([FromBody] OrderInforDTO orderInfor)
        {
            if (orderInfor == null || orderInfor.Amount <= 0)
            {
                return BadRequest("Dữ liệu đơn hàng không hợp lệ.");
            }

            try
            {
                var result = await _momoServices.CreatePaymentAsync(orderInfor);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }
        [HttpGet("by-order/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrderId(string orderId)
        {
            var result = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentById(string paymentId)
        {
            var result = await _paymentService.GetPaymentByPaymentdAsync(paymentId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        [HttpGet("momo-execute")]
        public IActionResult PaymentExecute()
        {
            try
            {
                var momoResponse = _momoServices.PaymentExecute(Request.Query);
                return Ok(momoResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("create-vnpay-payment")]
        public async Task<ActionResult<VnPayUrlResponseModel>> CreatePaymentUrlVnpay([FromBody] VnPaymentRequestModel model)
        {
            try
            {
                var url = await _vnPayService.CreateVNPayUrl(HttpContext, model);
                return Ok(new VnPayUrlResponseModel
                {
                    Success = true,
                    PaymentUrl = url,
                    Message = "Tạo URL thanh toán VNPay thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new VnPayUrlResponseModel
                {
                    Success = false,
                    PaymentUrl = null,
                    Message = ex.GetBaseException().Message
                });
            }
        }




        [HttpPost("{orderId}")]
        public async Task<IActionResult> AddPaymentToOrder(string orderId, [FromBody] CreatePaymentDTO dto)
        {
            try
            {
                var payment = await _paymentService.AddPaymentToOrderAsync(orderId, dto);
                return Ok(new { IsSuccess = true, Message = "Thêm thanh toán thành công", Data = payment });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }


        [HttpGet("PaymentCallbackVnpay")]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            try
            {
                var VNPayresponse = _vnPayService.PaymentExecute(Request.Query);

                if (!VNPayresponse.Success)
                {
                    return BadRequest(new { message = "Thanh toán không hợp lệ hoặc xác thực lỗi." });
                }

                var createPaymentDto = new CreatePaymentDTO
                {
                    Amount = VNPayresponse.Amount,
                    PaymentDate = DateTime.UtcNow,
                    Method = PaymentMethod.VNPay,
                    Status = PaymentStatus.Completed,
                    TransactionCode = VNPayresponse.TransactionId,
                    PaymentProvider = "VnPay",
                    Notes = VNPayresponse.OrderDescription
                };

                await _paymentService.AddPaymentToOrderAsync(VNPayresponse.OrderId, createPaymentDto);

                return Ok(VNPayresponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Đã xảy ra lỗi khi xử lý callback VNPay", error = ex.Message });
            }
        }
    }
}
