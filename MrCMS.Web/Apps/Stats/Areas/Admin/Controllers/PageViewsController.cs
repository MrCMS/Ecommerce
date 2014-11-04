using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Stats.Areas.Admin.Models;
using MrCMS.Web.Apps.Stats.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Stats.Areas.Admin.Controllers
{
    public class PageViewsController : MrCMSAppAdminController<StatsApp>
    {
        private readonly IPageViewAdminService _pageViewAdminService;

        public PageViewsController(IPageViewAdminService pageViewAdminService)
        {
            _pageViewAdminService = pageViewAdminService;
        }

        public ViewResult Index(PageViewSearchQuery query)
        {
            ViewData["results"] = _pageViewAdminService.Search(query);
            ViewData["search-type-options"] = _pageViewAdminService.GetSearchTypeOptions();
            return View(query);
        }

        public RedirectResult ShowPage(Webpage webpage)
        {
            return Redirect("~/" + webpage.LiveUrlSegment);
        }
    }
}