using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.Payments
{
    public interface IPaymentStatusService
    {
        List<SelectListItem> GetOptions();
    }
}