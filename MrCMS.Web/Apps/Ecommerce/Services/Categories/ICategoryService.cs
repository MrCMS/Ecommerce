using System.Collections.Generic;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Categories
{
    public interface ICategoryService
    {
        CategoryPagedList Search(string query = null, int page = 1);
        IEnumerable<AutoCompleteResult> Search(string query, List<int> ids);
    }
}