using MrCMS.Web.Apps.Ecommerce.Payment.Elavon.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Elavon.Services
{
    public interface IElavonPaymentService
    {
        ElavonPaymentDetailsModel GetElavonPaymentDetailsModel();
        string GetPaymentRequestResult();
        ElavonCustomResult CheckNotificationResult(string responseJson, out bool isSuccessNotification);
    }
}