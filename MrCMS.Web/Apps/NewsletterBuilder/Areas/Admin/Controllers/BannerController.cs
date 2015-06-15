using System.Web.Mvc;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.ActionFilters;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Controllers
{
    public class BannerController : MrCMSAppAdminController<NewsletterBuilderApp>
    {
        private readonly IContentItemAdminService _contentItemAdminService;

        public BannerController(IContentItemAdminService contentItemAdminService)
        {
            _contentItemAdminService = contentItemAdminService;
        }

        [HttpGet]
        [HandleMissingNewsletter]
        public ViewResult Add(int newsletterId = 0)
        {
            return View(_contentItemAdminService.GetNew<Banner>(newsletterId));
        }

        [HttpPost]
        [AddSuccessMessage("Banner successfully added")]
        public RedirectToRouteResult Add(Banner banner)
        {
            _contentItemAdminService.Add(banner);
            return RedirectToAction("Edit", "Newsletter", new { id = banner.Newsletter.Id });
        }

        [HttpGet]
        public ViewResult Edit(Banner banner)
        {
            return View(banner);
        }

        [HttpPost]
        [ActionName("Edit")]
        [AddSuccessMessage("Banner successfully updated")]
        public RedirectToRouteResult Edit_POST(Banner banner)
        {
            _contentItemAdminService.Update(banner);
            return RedirectToAction("Edit", "Newsletter", new { id = banner.Newsletter.Id });
        }
    }
}