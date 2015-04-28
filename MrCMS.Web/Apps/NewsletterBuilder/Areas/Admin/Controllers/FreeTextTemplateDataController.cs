using System.Web.Mvc;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.ActionFilters;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Controllers
{
    public class FreeTextTemplateDataController : MrCMSAppAdminController<NewsletterBuilderApp>
    {
        private readonly ITemplateDataAdminService _templateDataAdminService;

        public FreeTextTemplateDataController(ITemplateDataAdminService templateDataAdminService)
        {
            _templateDataAdminService = templateDataAdminService;
        }

        [HttpGet]
        [HandleMissingTemplate]
        public ViewResult Add(int templateId = 0)
        {
            var freeTextTemplateData = _templateDataAdminService.GetNew<FreeTextTemplateData>(templateId);
            return View(freeTextTemplateData);
        }

        [HttpPost]
        [AddSuccessMessage("Free text template data added")]
        public RedirectToRouteResult Add(FreeTextTemplateData data)
        {
            _templateDataAdminService.Add(data);
            return RedirectToAction("Edit", "NewsletterTemplate", new {id = data.NewsletterTemplate.Id});
        }

        [HttpGet]
        public ViewResult Edit(FreeTextTemplateData data)
        {
            return View(data);
        }

        [HttpPost]
        [ActionName("Edit")]
        [AddSuccessMessage("Free text template data updated")]
        public RedirectToRouteResult Edit_POST(FreeTextTemplateData data)
        {
            _templateDataAdminService.Update(data);
            return RedirectToAction("Edit", "NewsletterTemplate", new {id = data.NewsletterTemplate.Id});
        }
    }
}