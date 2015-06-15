using System.Web.Mvc;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.ActionFilters;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Controllers
{
    public class ImageRightAndTextLeftTemplateDataController : MrCMSAppAdminController<NewsletterBuilderApp>
    {
        private readonly ITemplateDataAdminService _templateDataAdminService;

        public ImageRightAndTextLeftTemplateDataController(ITemplateDataAdminService templateDataAdminService)
        {
            _templateDataAdminService = templateDataAdminService;
        }

        [HttpGet]
        [HandleMissingTemplate]
        public ViewResult Add(int templateId = 0)
        {
            var bannerTemplateData = _templateDataAdminService.GetNew<ImageRightAndTextLeftTemplateData>(templateId);
            return View(bannerTemplateData);
        }

        [HttpPost]
        [AddSuccessMessage("Image right and text left template data added")]
        public RedirectToRouteResult Add(ImageRightAndTextLeftTemplateData data)
        {
            _templateDataAdminService.Add(data);
            return RedirectToAction("Edit", "NewsletterTemplate", new {id = data.NewsletterTemplate.Id});
        }

        [HttpGet]
        public ViewResult Edit(ImageRightAndTextLeftTemplateData data)
        {
            return View(data);
        }

        [HttpPost]
        [ActionName("Edit")]
        [AddSuccessMessage("Image right and text left template data updated")]
        public RedirectToRouteResult Edit_POST(ImageRightAndTextLeftTemplateData data)
        {
            _templateDataAdminService.Update(data);
            return RedirectToAction("Edit", "NewsletterTemplate", new {id = data.NewsletterTemplate.Id});
        }
    }
}