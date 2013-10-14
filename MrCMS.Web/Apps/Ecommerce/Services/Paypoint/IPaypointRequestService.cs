using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Paypoint
{
    public interface IPaypointRequestService
    {
        ProcessDetailsResponse ProcessStandardTransaction(PaypointPaymentDetailsModel model);
        ProcessDetailsResponse Process3DSecureTransaction(PaypointPaymentDetailsModel model, string threeDSecureUrl);
        ProcessDetailsResponse Handle3DSecureResponse(FormCollection formCollection);
    }
}