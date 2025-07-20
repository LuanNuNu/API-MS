namespace MenShop_Assignment.Models.VNPay
{
    public class VnPaymentRequestModel
    {
        public string OrderId { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}
