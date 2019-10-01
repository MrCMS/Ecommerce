using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Elavon.Services
{
    public interface IElavonPaymentService
    {
        string BuildChargeRequest(out string chargeRequestResult);
         
        ActionResult HandleNotification(string responseJson);
    }
}