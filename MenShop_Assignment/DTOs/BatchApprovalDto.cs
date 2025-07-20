namespace MenShop_Assignment.DTOs
{
    public class BatchApprovalDto
    {
        public int Total { get; set; }
        public int Approved { get; set; }
        public int Failed { get; set; }
        public List<string> ApprovedIds { get; set; }
        public List<FailedOrderDto> FailedOrders { get; set; }
    }
}
