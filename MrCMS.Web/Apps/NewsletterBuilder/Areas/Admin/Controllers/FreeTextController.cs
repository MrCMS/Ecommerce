using System.Web.Mvc;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.ActionFilters;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Controllers
{
    public class FreeTextController : MrCMSAppAdminController<NewsletterBuilderApp>
    {
        private readonly IContentItemAdminService _contentItemAdminService;

        public FreeTextController(IContentItemAdminService contentItemAdminService)
        {
            _contentItemAdminService = contentItemAdminService;
        }

        [HttpGet]
        [HandleMissingNewsletter]
        public ViewResult Add(int newsletterId = 0)
        {
            return View(_contentItemAdminService.GetNew<FreeText>(newsletterId));
        }

        [HttpPost]
        [AddSuccessMessage("Free Text successfully added")]
        public RedirectToRouteResult Add(FreeText freeText)
        {
            _contentItemAdminService.Add(freeText);
            return RedirectToAction("Edit", "Newsletter", new { id = freeText.Newsletter.Id });
        }

        [HttpGet]
        public ViewResult Edit(FreeText freeText)
        {
            return View(freeText);
        }

        [HttpPost]
        [ActionName("Edit")]
        [AddSuccessMessage("Free Text successfully updated")]
        public RedirectToRouteResult Edit_POST(FreeText freeText)
        {
            _contentItemAdminService.Update(freeText);
            return RedirectToAction("Edit", "Newsletter", new { id = freeText.Newsletter.Id });
        }
    }
}