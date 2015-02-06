using System.Web;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Payment.CharityClear.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment.CharityClear.Services
{
    public interface ICharityClearPaymentService
    {
        CharityClearPostModel GetInfo();
        CharityClearResponse HandleNotification(FormCollection form);
    }
}