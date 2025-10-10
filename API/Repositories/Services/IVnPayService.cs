using API.Models.DTOs;

namespace API.Repositories.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model, string? returnUrl = null);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
