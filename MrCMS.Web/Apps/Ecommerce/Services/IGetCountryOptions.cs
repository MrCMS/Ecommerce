using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IGetCountryOptions
    {
        List<SelectListItem> Get();
    }
}