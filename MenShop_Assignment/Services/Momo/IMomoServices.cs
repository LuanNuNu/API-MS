using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models.Momo;

namespace MenShop_Assignment.Services.Momo
{
    public interface IMomoServices
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInforDTO model);
        MomoExecuteResponseModel PaymentExecute(IQueryCollection colection);
    }
}
