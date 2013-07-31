using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.Misc
{
    public interface IOptionService
    {
        List<SelectListItem> GetEnumOptions<T>() where T : struct;
        IList<SelectListItem> GetCategoryOptions();
    }
}