using MenShop_Assignment.Models.VNPay;

namespace MenShop_Assignment.Services.VNPay
{
    public interface IVNPayService
    {
        Task<string> CreateVNPayUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
}
