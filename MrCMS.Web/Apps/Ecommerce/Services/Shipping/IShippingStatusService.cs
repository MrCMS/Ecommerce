using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public interface IShippingStatusService
    {
        List<SelectListItem> GetOptions();
    }
}