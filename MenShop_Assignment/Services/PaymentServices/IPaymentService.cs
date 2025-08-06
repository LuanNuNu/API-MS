using MenShop_Assignment.Datas;
using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;
using MenShop_Assignment.Models.Momo;

namespace MenShop_Assignment.Services.PaymentServices
{
    public interface IPaymentService
    {
        Task<PaymentViewModel> AddPaymentToOrderAsync(string orderId, CreatePaymentDTO dto);
        Task<PaymentViewModel?> GetPaymentByPaymentdAsync(string paymentId);
        Task<PaymentViewModel?> GetPaymentByOrderIdAsync(string orderId);
    }
}
