using MenShop_Assignment.Extensions;

namespace MenShop_Assignment.Datas
{
    public class Order
    {
        public string? OrderId { get; set; }
        public string? CustomerId { get; set; }
        public string? EmployeeId { get; set; }
        public string? ShipperId { get; set; }
        public User? Shipper { get; set; }
        public User? Employee { get; set; }
        public User? Customer { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public OrderStatus? Status { get; set; }
        public bool? IsOnline { get; set; }
        public decimal? Total {  get; set; }
        public string? Address { get; set; }
        public ICollection<OrderDetail>? Details { get; set; }
		public ICollection<Payment>? Payments { get; set; }
	}
}
