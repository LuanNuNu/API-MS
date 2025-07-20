using Castle.Components.DictionaryAdapter.Xml;
using MenShop_Assignment.Datas;
using MenShop_Assignment.Models.VNPay;
using Microsoft.EntityFrameworkCore;
using VNPAY_CS_ASPX;

namespace MenShop_Assignment.Services.VNPay
{
    public class VNPayService : IVNPayService
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        public VNPayService(IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _config = configuration;
            _context = dbContext;
        }
        public async Task<string> CreateVNPayUrl(HttpContext context, VnPaymentRequestModel model)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == model.OrderId);
            if (order == null)
                throw new KeyNotFoundException("Không tìm thấy đơn hàng.");

            var totalAmount = order.Total;

            if (model.Amount <= 0)
                throw new ArgumentException("Số tiền thanh toán phải lớn hơn 0.");

            if (model.Amount > totalAmount)
                throw new ArgumentException("Số tiền thanh toán không được lớn hơn tổng tiền đơn hàng.");


            //
            var tick = DateTime.Now.Ticks.ToString();

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString()); 

            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toán cho đơn hàng: {model.OrderId}, Khách hàng: {model.FullName}");
            vnpay.AddRequestData("vnp_OrderType", "other"); 
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackReturnUrl"]);

            vnpay.AddRequestData("vnp_TxnRef", model.OrderId); 

            var paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);

            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = vnpay.GetResponseData("vnp_TxnRef"); 
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            var vnp_Amount = vnpay.GetResponseData("vnp_Amount");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false
                };
            }

            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "VnPay",
                Amount = decimal.Parse(vnp_Amount) / 100,
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_orderId,
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode
            };
        }


    }
}
