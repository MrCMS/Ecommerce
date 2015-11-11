using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IGetPricingMethodOptions
    {
        List<SelectListItem> GetOptions();
    }
}