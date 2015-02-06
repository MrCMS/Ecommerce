using System.Collections.Generic;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface ICategoryAdminService
    {
        CategoryPagedList Search(string searchTerm, int page = 1);
        IEnumerable<AutoCompleteResult> Search(string query, List<int> ids);
        bool ProductContainerExists();
    }
}