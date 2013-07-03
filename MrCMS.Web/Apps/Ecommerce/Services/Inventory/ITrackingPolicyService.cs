using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory
{
    public interface ITrackingPolicyService
    {
        List<SelectListItem> GetOptions();
    }
}