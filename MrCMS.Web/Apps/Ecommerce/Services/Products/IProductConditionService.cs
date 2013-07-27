using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IProductConditionService
    {
        List<SelectListItem> GetOptions();
    }
}