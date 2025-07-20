using MenShop_Assignment.Models.Momo;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;
using System.Text;
using RestSharp;
using Newtonsoft.Json;
using MenShop_Assignment.DTOs;

namespace MenShop_Assignment.Services.Momo
{
    public class MomoServices : IMomoServices
    {
        private readonly MomoOptionModel _options;
        public MomoServices(MomoOptionModel optionModel)
        {
            _options = optionModel;
        }

        public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInforDTO model)
        {
            model.OrderInformation = $"Khách hàng: {model.FullName}. Nội dung: {model.OrderInformation}";
            var rawData =
                $"partnerCode={_options.PartnerCode}" +
                $"&accessKey={_options.AccessKey}" +
                $"&requestId={model.OrderId}" +
                $"&amount={model.Amount}" +
                $"&orderId={model.OrderId}" +
                $"&orderInfo={model.OrderInformation}" +
                $"&returnUrl={_options.ReturnUrl}" +
                $"&notifyUrl={_options.NotifyUrl}" +
                $"&extraData=";

            var signature = ComputeHmacSha256(rawData, _options.SecretKey);
            var client = new RestClient(_options.MomoApiUrl);
            var request = new RestRequest("", Method.Post);
            var requestData = new
            {
                accessKey = _options.AccessKey,
                partnerCode = _options.PartnerCode,
                requestType = _options.RequestType,
                notifyUrl = _options.NotifyUrl,
                returnUrl = _options.ReturnUrl,
                orderId = model.OrderId,
                amount = model.Amount.ToString(),
                orderInfo = model.OrderInformation,
                requestId = model.OrderId,
                extraData = "",
                signature = signature
            };

            // Không cần JsonConvert ở đây nếu dùng AddJsonBody
            request.AddJsonBody(requestData);

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                throw new Exception($"Lỗi gọi API Momo: {response.StatusCode} - {response.Content}");
            }

            if (string.IsNullOrEmpty(response.Content))
            {
                throw new Exception("Không nhận được phản hồi từ Momo.");
            }
            return JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);
        }

        public MomoExecuteResponseModel PaymentExecute(IQueryCollection collection)
        {
            collection.TryGetValue("amount", out var amount);
            collection.TryGetValue("orderInfomation", out var orderInfo);
            collection.TryGetValue("orderId", out var orderId);

            return new MomoExecuteResponseModel()
            {
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo
            };
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return hashString;
        }

    }
}
