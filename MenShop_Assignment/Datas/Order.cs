using MenShop_Assignment.Extensions;

namespace MenShop_Assignment.Datas
{
    public class Order
    {
        public string? OrderId { get; set; }
        public string? CustomerId { get; set; }
        public string? EmployeeId { get; set; }

        public int? BranchId { get; set; } 
        public Branch? Branch { get; set; }

        public string? ShipperId { get; set; }
        public User? Shipper { get; set; }
        public User? Employee { get; set; }
        public User? Customer { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public OrderStatus? Status { get; set; }
        public bool? IsOnline { get; set; }

        public decimal? Subtotal { get; set; } 

        public decimal? ShippingFee { get; set; } 

        public decimal? Total => Subtotal + ShippingFee;
        //
        public string? Address { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverPhone { get; set; }
        public string? CancellationReason { get; set; }
        public ICollection<OrderDetail>? Details { get; set; }
		public ICollection<Payment>? Payments { get; set; }
	}
}
