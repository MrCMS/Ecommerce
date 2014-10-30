using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Paging;
using MrCMS.Web.Apps.Stats.Areas.Admin.Models;

namespace MrCMS.Web.Apps.Stats.Areas.Admin.Services
{
    public interface IPageViewAdminService
    {
        IPagedList<PageViewResult> Search(PageViewSearchQuery query);
        List<SelectListItem> GetSearchTypeOptions();
    }
}