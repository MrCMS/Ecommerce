using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.Users
{
    public interface IGenderService
    {
        List<SelectListItem> GetOptions();
    }
}