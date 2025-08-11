    using Castle.Core.Resource;
using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Extensions;
using MenShop_Assignment.Models;
using MenShop_Assignment.Repositories.OrderRepository;
using MenShop_Assignment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MenShop_Assignment.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAutoOrderService _orderService;
        public OrderController(IOrderRepository shipperRepository, IAutoOrderService autoOrderService)
        {
            _orderRepository = shipperRepository;
            _orderService = autoOrderService;
        }

        [HttpGet("getall-orders")]
        [Authorize]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = _orderRepository.GetOrdersAsync(null).Result.ToList();
            return Ok(result);
        }
        [HttpGet("getall-onlineorders")]
        [Authorize]
        public async Task<IActionResult> GetAllOnlineOrders()
        {
            var onlineOrders =  _orderRepository.GetOrdersAsync(new SearchOrderDTO { IsOnline= true}).Result.ToList();
            return Ok(onlineOrders);
        }
        [HttpGet("get-ordersId/{orderId}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById(string orderId)
        {
            var orders = await _orderRepository.GetOrdersAsync(new SearchOrderDTO { OrderId = orderId });
            var order = orders.FirstOrDefault();
            if (order == null) return NotFound();
            return Ok(order); 
        }




        [HttpGet("get-orders/{shipperId}")]
        [Authorize]
        public async Task<IActionResult> GetOrdersByShipperId(string shipperId)
        {
            var result =  _orderRepository.GetOrdersAsync(new SearchOrderDTO { ShipperId = shipperId}).Result.ToList();
            return Ok(result);
        }

        [HttpGet("getorders-by-district")]
        [Authorize]
        public async Task<ActionResult<ApiResponseModel<List<OrderViewModel>>>> GetOrdersByDistrict(string district)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersAsync(new SearchOrderDTO { District = district });

                var response = new ApiResponseModel<List<OrderViewModel>>(
                    isSuccess: true,
                    message: $"Tìm thấy {orders.Count} đơn hàng ở quận {district}",
                    data: orders.ToList(),
                    statusCode: 200
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponseModel<List<OrderViewModel>>(
                    isSuccess: false,
                    message: ex.Message,
                    data: null,
                    statusCode: 500
                );

                return StatusCode(500, errorResponse);
            }
        }



        [HttpPut("auto-approve/{orderId}")]
        [Authorize]
        public async Task<IActionResult> AutoApprove(string orderId)
        {
            var result = await _orderService.ApproveOnlineOrderAsync(orderId);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }
        [HttpPut("auto-decrease-stock/{orderId}")]
        [Authorize]
        public async Task<IActionResult> AutoDecreaseStockOffline(string orderId)
        {
            var result = await _orderService.ApproveOfflineOrderAsync(orderId);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("pending")]
        [Authorize]
        public async Task<IActionResult> GetPending()
        {
            var result = await _orderRepository.GetOrdersAsync(
                new SearchOrderDTO
                {
                    Status = OrderStatus.Pending 
                });

            return Ok(result);
        }


        //Order for Customer 

        [HttpGet("get-order-by-customerId")]
        [Authorize]
        public async Task<ActionResult> GetOrdersByCustomerId(string? customerId)
		{
			try
			{
				var orders =  _orderRepository.GetOrdersAsync(new SearchOrderDTO { CustomerId= customerId}).Result.ToList();

				return Ok(orders);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Lỗi: {ex.Message}");
			}
		}

        [HttpGet("get-order-detail/{orderId}")]
        [Authorize]
        public async Task<ActionResult> GetOrderDetailById(string orderId)
        {
            try
            {
                if (string.IsNullOrEmpty(orderId))
                    return BadRequest("Mã đơn hàng không hợp lệ");

                var orders = await _orderRepository.GetOrderDetailsByOrderIdsAsync(orderId);

                if (orders == null)
                    return NotFound("Không tìm thấy đơn hàng");

                return Ok(orders); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }



        [HttpGet("search")]
        [Authorize]
        public async Task<ActionResult> SearchOrders([FromBody] SearchOrderDTO? searchDto)
		{
            var orderList = _orderRepository.GetOrdersAsync(searchDto).Result;
			return Ok(orderList);
		}

        [HttpPut("cancel/{orderId}")]
        [Authorize]
        public async Task<IActionResult> CancelOrder(string orderId, string reason)
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                return BadRequest(new ApiResponseModel<object>(
                    false,
                    "Mã đơn hàng không hợp lệ.",
                    null,
                    400
                ));
            }

            var result = await _orderRepository.CancelOrderAsync(orderId, reason);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("complete/{orderId}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseModel<object>>> CompleteOrder(string orderId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orderId))
                {
                    return BadRequest(new ApiResponseModel<object>(
                        false,
                        "Mã đơn hàng không hợp lệ",
                        null,
                        400
                    ));
                }

                var result = await _orderRepository.CompletedOrderStatus(orderId);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseModel<object>(
                    false,
                    "Đã xảy ra lỗi hệ thống",
                    null,
                    500,
                    new List<string> { ex.Message }
                ));
            }
        }
        [HttpPut("updateOrder-shipperStatus")]
        [Authorize]
        public async Task<IActionResult> ShipperAcceptOrder([FromQuery] string orderId, [FromQuery] string shipperId)
        {
            var result = await _orderRepository.ShipperAcceptOrderByOrderId(orderId, shipperId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("createOrder")]
        [Authorize]
        public async Task<ActionResult<OrderResponseDTO>> CreateOrderAsync([FromBody] CreateOrderDTO createOrderDto)
        {
            if (createOrderDto == null)
            {
                return BadRequest("Dữ liệu không hợp lệ. Vui lòng cung cấp đầy đủ thông tin yêu cầu.");
            }

            try
            {
                var orderResponse = await _orderRepository.CreateOrderAsync(createOrderDto);
                return Ok(orderResponse);

            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Có lỗi khi tạo hóa đơn: {ex.Message}");
            }
        }

    }
}
