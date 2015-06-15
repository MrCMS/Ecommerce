using System.Web.Mvc;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.ActionFilters;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Controllers
{
    public class BannerTemplateDataController : MrCMSAppAdminController<NewsletterBuilderApp>
    {
         private readonly ITemplateDataAdminService _templateDataAdminService;

        public BannerTemplateDataController(ITemplateDataAdminService templateDataAdminService)
        {
            _templateDataAdminService = templateDataAdminService;
        }

        [HttpGet]
        [HandleMissingTemplate]
        public ViewResult Add(int templateId = 0)
        {
            var bannerTemplateData = _templateDataAdminService.GetNew<BannerTemplateData>(templateId);
            return View(bannerTemplateData);
        }

        [HttpPost]
        [AddSuccessMessage("Banner template data added")]
        public RedirectToRouteResult Add(BannerTemplateData data)
        {
            _templateDataAdminService.Add(data);
            return RedirectToAction("Edit", "NewsletterTemplate", new {id = data.NewsletterTemplate.Id});
        }

        [HttpGet]
        public ViewResult Edit(BannerTemplateData data)
        {
            return View(data);
        }

        [HttpPost]
        [ActionName("Edit")]
        [AddSuccessMessage("Banner template data updated")]
        public RedirectToRouteResult Edit_POST(BannerTemplateData data)
        {
            _templateDataAdminService.Update(data);
            return RedirectToAction("Edit", "NewsletterTemplate", new {id = data.NewsletterTemplate.Id});
        }
    }
}