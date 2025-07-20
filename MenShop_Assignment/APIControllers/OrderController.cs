using MenShop_Assignment.Repositories.OrderRepository;
using MenShop_Assignment.Models;
using MenShop_Assignment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MenShop_Assignment.DTOs;

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

        [HttpGet("getallorders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = _orderRepository.GetOrdersAsync(null).Result.ToList();
            return Ok(result);
        }
        [HttpGet("getallonlineorders")]
        public async Task<IActionResult> GetAllOnlineOrders()
        {
            var onlineOrders =  _orderRepository.GetOrdersAsync(new SearchOrderDTO { IsOnline= true}).Result.ToList();
            return Ok(onlineOrders);
        }
        [HttpPut("UpdateOrderShipperStatus")]
        public async Task<IActionResult> UpdateOrderShipperStatus(string orderId, string shipperId)
        {
            await _orderRepository.ShipperAcceptOrderByOrderId(orderId,shipperId);
            return NoContent();
        }
        [HttpPut("CompletedOrder")]
        public async Task<IActionResult> CompleteOrderStatus(string orderId)
        {
            await _orderRepository.CompletedOrderStatus(orderId);
            return NoContent();
        }
        [HttpGet("getorders")]
        public async Task<IActionResult> GetOrdersByShipperId(string shipperId)
        {
            var result =  _orderRepository.GetOrdersAsync(new SearchOrderDTO { ShipperId = shipperId}).Result.ToList();
            return Ok(result);
        }
        [HttpGet("getordersbyaddress")]
        public async Task<IActionResult> GetOrdersByShipperAddress()
        {
            return NoContent();
        }
        [HttpGet("getordersbydistrict")]
        public async Task<ActionResult<List<OrderViewModel>>> GetOrdersByDistrict(string district)
        {
            var orders =  _orderRepository.GetOrdersAsync(new SearchOrderDTO { District = district}).Result.ToList();
            return Ok(orders);
        }
 
        [HttpPut("auto-approve")]
        public async Task<IActionResult> AutoApproveAll()
        {
            var result = await _orderService.ApproveAllOrdersAsync();
            return Ok(result);
        }

        [HttpPut("auto-approve/{orderId}")]
        public async Task<IActionResult> AutoApprove(string orderId)
        {
            var result = await _orderService.ApproveOrderAsync(orderId);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _orderService.GetPendingOrdersAsync();
            return Ok(result);
        }

		//Order for Customer 

		[HttpGet("getorder")]
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

        [HttpGet("getorderdetail/{orderId}")]
        public async Task<ActionResult> GetOrderById(string orderId)
        {
            try
            {
                if (string.IsNullOrEmpty(orderId))
                    return BadRequest("Mã đơn hàng không hợp lệ");

                var order = await _orderRepository.GetOrdersAsync(new SearchOrderDTO { OrderId = orderId });
                if (order == null)
                    return NotFound("Không tìm thấy đơn hàng");

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }


        [HttpGet("search")]
		public async Task<ActionResult> SearchOrders([FromBody] SearchOrderDTO? searchDto)
		{
            var orderList = _orderRepository.GetOrdersAsync(searchDto).Result;
			return Ok(orderList);
		}

		[HttpPut("cancel")]
		public async Task<ActionResult> CancelOrder([FromBody] string cancelDto)
		{
			try
			{
				if (cancelDto == null )
					return BadRequest("Mã đơn hàng không hợp lệ");
				var result = await _orderRepository.CancelOrderAsync(cancelDto);

				return result ? Ok("Hủy đơn hàng thành công") : BadRequest("Không thể hủy đơn hàng");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Lỗi: {ex.Message}");
			}
		}


        [HttpPost("createOrder")]
        public async Task<ActionResult<OrderViewModel>> CreateOrderAsync([FromBody] CreateOrderDTO createOrderDto)
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
