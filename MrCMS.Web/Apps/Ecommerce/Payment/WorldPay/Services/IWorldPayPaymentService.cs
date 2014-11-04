using System.Web;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Payment.WorldPay.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment.WorldPay.Services
{
    public interface IWorldPayPaymentService
    {
        WorldPayPostInfo GetInfo();
        ActionResult HandleNotification(HttpRequestBase request);
    }
}