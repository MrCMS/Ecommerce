using System.Web.Mvc;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.ActionFilters;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Controllers
{
    public class ImageLeftAndTextRightController : MrCMSAppAdminController<NewsletterBuilderApp>
    {
        private readonly IContentItemAdminService _contentItemAdminService;

        public ImageLeftAndTextRightController(IContentItemAdminService contentItemAdminService)
        {
            _contentItemAdminService = contentItemAdminService;
        }

        [HttpGet]
        [HandleMissingNewsletter]
        public ViewResult Add(int newsletterId = 0)
        {
            return View(_contentItemAdminService.GetNew<ImageLeftAndTextRight>(newsletterId));
        }

        [HttpPost]
        [AddSuccessMessage("Image Left and Text Right successfully added")]
        public RedirectToRouteResult Add(ImageLeftAndTextRight imageLeftAndTextRight)
        {
            _contentItemAdminService.Add(imageLeftAndTextRight);
            return RedirectToAction("Edit", "Newsletter", new { id = imageLeftAndTextRight.Newsletter.Id });
        }

        [HttpGet]
        public ViewResult Edit(ImageLeftAndTextRight imageLeftAndTextRight)
        {
            return View(imageLeftAndTextRight);
        }

        [HttpPost]
        [ActionName("Edit")]
        [AddSuccessMessage("Image Left and Text Right successfully updated")]
        public RedirectToRouteResult Edit_POST(ImageLeftAndTextRight imageLeftAndTextRight)
        {
            _contentItemAdminService.Update(imageLeftAndTextRight);
            return RedirectToAction("Edit", "Newsletter", new { id = imageLeftAndTextRight.Newsletter.Id });
        }
    }
}