using MenShop_Assignment.Models;

namespace MenShop_Assignment.DTOs
{
    public class PendingOrdersDto
    {
        public int Count { get; set; }
        public List<OrderViewModel>? Orders { get; set; }
    }
}
