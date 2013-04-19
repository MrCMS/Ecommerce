using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public interface IShippingStatusService
    {
        List<SelectListItem> GetOptions();
    }
}