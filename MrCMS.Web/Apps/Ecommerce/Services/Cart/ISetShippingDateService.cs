using System;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface ISetShippingDateService
    {
        void SetDate(DateTime date);
        ActionResult RedirectToSetShippingDetails();
    }
}