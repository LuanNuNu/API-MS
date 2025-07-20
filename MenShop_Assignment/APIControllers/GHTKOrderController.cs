using MenShop_Assignment.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MenShop_Assignment.APIControllers
{
    public class GHTKOrderController : ControllerBase
    {
        private readonly IGHTKOrderRepository _orderRepository;
        public GHTKOrderController(IGHTKOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        [HttpGet("GHTKGet")]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderRepository.GetAllGHTKOrdersAsync();
            return Ok(result);
        }
    }
}
