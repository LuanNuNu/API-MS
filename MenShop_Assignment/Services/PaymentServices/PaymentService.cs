using MenShop_Assignment.Datas;
using MenShop_Assignment.Extensions;
using MenShop_Assignment.Repositories.OrderRepository;
using MenShop_Assignment.Services.Momo;
using Microsoft.EntityFrameworkCore;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Mapper;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Services.PaymentServices
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMomoServices _momoServices;
        private readonly IOrderRepository _orderRepository;
        public PaymentService(ApplicationDbContext context, IMomoServices momoServices, IOrderRepository orderRepository)
        {
            _context = context;
            _momoServices = momoServices;
            _orderRepository = orderRepository;
        }

        public async Task<PaymentViewModel?> GetPaymentByOrderIdAsync(string orderId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == orderId);

            if (payment == null)
            {
                return null;
            }

            return PaymentMapper.ToPaymentViewModel(payment);
        }
        public async Task<PaymentViewModel?> GetPaymentByPaymentdAsync(string paymentId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
            {
                return null;
            }

            return PaymentMapper.ToPaymentViewModel(payment);
        }
        public async Task<PaymentViewModel> AddPaymentToOrderAsync(string orderId, CreatePaymentDTO dto)
        {
            var order = await _context.Orders
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                throw new Exception("Không tìm thấy đơn hàng.");
            ApplyPaymentMethodInfo(dto);


            var payment = new Payment
            {
                PaymentId = "PM" + Guid.NewGuid().ToString("N").Substring(0, 12),
                OrderId = orderId,
                Amount = dto.Amount,
                PaymentDate = dto.PaymentDate,
                Method = dto.Method,
                Status = dto.Status,
                TransactionCode = dto.TransactionCode,
                PaymentProvider = dto.PaymentProvider,
                Notes = dto.Notes,
                StaffId = order.EmployeeId,
                Discounts = dto.Discounts?.Select(d => new PaymentDiscount
                {
                    CouponCode = d.CouponCode,
                    DiscountAmount = d.DiscountAmount,
                    DiscountPercentage = d.DiscountPercentage,
                    Type = d.Type,
                }).ToList()
            };

            payment.Order = order;
            order.Payments.Add(payment);

            await HandleOrderStatusAfterPayment(order, payment, dto.Method);
            await _context.SaveChangesAsync();
            await _context.Entry(payment).Collection(p => p.Discounts).LoadAsync();

            return PaymentMapper.ToPaymentViewModel(payment);
        }
        private void ApplyPaymentMethodInfo(CreatePaymentDTO dto)
        {
            switch (dto.Method)
            {
                case PaymentMethod.Cash:
                    dto.PaymentDate = DateTime.Now;
                    dto.Status = PaymentStatus.Completed;
                    dto.PaymentProvider = "Cash";
                    dto.TransactionCode = null;
                    dto.Notes = "Thanh toán bằng tiền mặt tại quầy";
                    break;

                case PaymentMethod.VNPay:
                    if (string.IsNullOrEmpty(dto.TransactionCode))
                        throw new Exception("Thiếu mã giao dịch cho VNPay.");
                    dto.PaymentProvider = "VNPay";
                    break;

                case PaymentMethod.COD:
                    dto.Status = PaymentStatus.Pending;
                    dto.PaymentProvider = "COD";
                    dto.TransactionCode = null;
                    dto.Notes = "Thanh toán khi nhận hàng";


                    break;

                default:
                    throw new Exception("Phương thức thanh toán không hợp lệ.");
            }
        }

        private async Task HandleOrderStatusAfterPayment(Order order, Payment payment, PaymentMethod method)
        {
            var totalPaid = order.Payments
                  .Where(p => p.Method != PaymentMethod.COD || p.Status == PaymentStatus.Completed)
                  .Sum(p => p.Amount);

            if (totalPaid > order.Total)
                throw new Exception("Tổng tiền thanh toán vượt quá số tiền đơn hàng.");

            if (order.IsOnline == false)
            {
                if (totalPaid == order.Total && payment.Status == PaymentStatus.Completed)
                {
                    order.Status = OrderStatus.Completed;
                    order.CompletedDate = DateTime.Now;
                    order.PaidDate = DateTime.Now;
                }
            }
            if (order.IsOnline == true)
            {
                if (method == PaymentMethod.COD)
                {
                    payment.Status = PaymentStatus.Pending;

                }
                else if (method == PaymentMethod.VNPay)
                {
                    payment.Status = PaymentStatus.Completed;
                    payment.PaymentDate = DateTime.Now;
                    order.PaidDate = DateTime.Now;
                    order.Status = OrderStatus.Paid;
                }

            }
        }
    }

}

