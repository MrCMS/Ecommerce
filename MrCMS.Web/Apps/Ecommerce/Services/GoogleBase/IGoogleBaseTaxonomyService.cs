using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.GoogleBase
{
    public interface IGoogleBaseTaxonomyService
    {
        List<SelectListItem> GetOptions();
    }
}