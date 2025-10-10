using API.Models.DTOs;
using API.Repositories.Services;
using API.Helpers;
namespace API.Repositories
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;

        public VnPayService(IConfiguration config)
        {
            _config = config;
        }
        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model, string? returnUrl = null)
        {
            var vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", _config["Vnpay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["Vnpay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["Vnpay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["Vnpay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["Vnpay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", model.Description);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", returnUrl ?? _config["Vnpay:PaymentBackReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", model.OrderId.ToString());

            return vnpay.CreateRequestUrl(
                _config["Vnpay:BaseUrl"],
                _config["Vnpay:HashSecret"]
            );
        }


        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnapy = new VnPayLibrary();
            foreach (var (key,value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnapy.AddResponseData(key, value.ToString());
                }
            }
            var vnp_orderId = Convert.ToInt64(vnapy.GetResponseData("vnp_TxnRef"));
            var vnp_transactionNo = Convert.ToInt64(vnapy.GetResponseData("vnp_TransactionNo"));
            var vnp_secureHash = collections.FirstOrDefault(x => x.Key == "vnp_SecureHash").Value;
            var vnp_responseCode = vnapy.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnapy.GetResponseData("vnp_OrderInfo");

            bool checkSignature = vnapy.ValidateSignature(vnp_secureHash, _config["Vnpay:HashSecret"]);
            if(!checkSignature)
            {
                return new VnPaymentResponseModel()
                {
                    Success = false
                };
            }
            return new VnPaymentResponseModel()
            {
                Success = true,
                OrderId = vnp_orderId.ToString(),
                //PaymentId = vnp_transactionNo.ToString(),
                VnPayResponseCode = vnp_responseCode,
                OrderDescription = vnp_OrderInfo,
                PaymentMethod = "VnPay",
                TransactionId = vnp_transactionNo.ToString(),
                Token = vnp_secureHash
            };



        }
    }
}
