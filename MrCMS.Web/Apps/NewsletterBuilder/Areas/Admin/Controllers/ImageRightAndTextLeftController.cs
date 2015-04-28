using System.Web.Mvc;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.ActionFilters;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Controllers
{
    public class ImageRightAndTextLeftController : MrCMSAppAdminController<NewsletterBuilderApp>
    {
        private readonly IContentItemAdminService _contentItemAdminService;

        public ImageRightAndTextLeftController(IContentItemAdminService contentItemAdminService)
        {
            _contentItemAdminService = contentItemAdminService;
        }

        [HttpGet]
        [HandleMissingNewsletter]
        public ViewResult Add(int newsletterId = 0)
        {
            return View(_contentItemAdminService.GetNew<ImageRightAndTextLeft>(newsletterId));
        }

        [HttpPost]
        [AddSuccessMessage("Image Right and Text Left successfully added")]
        public RedirectToRouteResult Add(ImageRightAndTextLeft imageRightAndTextLeft)
        {
            _contentItemAdminService.Add(imageRightAndTextLeft);
            return RedirectToAction("Edit", "Newsletter", new { id = imageRightAndTextLeft.Newsletter.Id });
        }

        [HttpGet]
        public ViewResult Edit(ImageRightAndTextLeft imageRightAndTextLeft)
        {
            return View(imageRightAndTextLeft);
        }

        [HttpPost]
        [ActionName("Edit")]
        [AddSuccessMessage("Image Right and Text Left successfully updated")]
        public RedirectToRouteResult Edit_POST(ImageRightAndTextLeft imageRightAndTextLeft)
        {
            _contentItemAdminService.Update(imageRightAndTextLeft);
            return RedirectToAction("Edit", "Newsletter", new { id = imageRightAndTextLeft.Newsletter.Id });
        }
    }
}