using System.Web.Mvc;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.ActionFilters;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Controllers
{
    public class ImageLeftAndTextRightTemplateDataController : MrCMSAppAdminController<NewsletterBuilderApp>
    {
        private readonly ITemplateDataAdminService _templateDataAdminService;

        public ImageLeftAndTextRightTemplateDataController(ITemplateDataAdminService templateDataAdminService)
        {
            _templateDataAdminService = templateDataAdminService;
        }

        [HttpGet]
        [HandleMissingTemplate]
        public ViewResult Add(int templateId = 0)
        {
            var bannerTemplateData = _templateDataAdminService.GetNew<ImageLeftAndTextRightTemplateData>(templateId);
            return View(bannerTemplateData);
        }

        [HttpPost]
        [AddSuccessMessage("Image left and text right template data added")]
        public RedirectToRouteResult Add(ImageLeftAndTextRightTemplateData data)
        {
            _templateDataAdminService.Add(data);
            return RedirectToAction("Edit", "NewsletterTemplate", new {id = data.NewsletterTemplate.Id});
        }

        [HttpGet]
        public ViewResult Edit(ImageLeftAndTextRightTemplateData data)
        {
            return View(data);
        }

        [HttpPost]
        [ActionName("Edit")]
        [AddSuccessMessage("Image left and text right template data updated")]
        public RedirectToRouteResult Edit_POST(ImageLeftAndTextRightTemplateData data)
        {
            _templateDataAdminService.Update(data);
            return RedirectToAction("Edit", "NewsletterTemplate", new {id = data.NewsletterTemplate.Id});
        }
    }
}