using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services.Payments
{
    public interface IPaymentStatusService
    {
        List<SelectListItem> GetOptions();
    }
}