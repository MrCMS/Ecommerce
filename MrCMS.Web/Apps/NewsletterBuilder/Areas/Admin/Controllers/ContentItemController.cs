using System.Web.Mvc;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.ActionFilters;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Controllers
{
    public class ContentItemController : MrCMSAppAdminController<NewsletterBuilderApp>
    {
        private readonly IContentItemAdminService _contentItemAdminService;

        public ContentItemController(IContentItemAdminService contentItemAdminService)
        {
            _contentItemAdminService = contentItemAdminService;
        }

        [HttpGet]
        public ViewResult Delete(ContentItem item)
        {
            return View(item);
        }

        [HttpPost]
        [ActionName("Delete")]
        [AddSuccessMessage("Content Item deleted")]
        public RedirectToRouteResult Delete_POST(ContentItem item)
        {
            _contentItemAdminService.Delete(item);
            return RedirectToAction("Edit", "Newsletter", new {id = item.Newsletter.Id});
        }
    }
}