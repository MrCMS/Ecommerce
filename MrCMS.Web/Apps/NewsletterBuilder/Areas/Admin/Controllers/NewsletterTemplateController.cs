using System.Web.Mvc;
using MrCMS.Web.Apps.NewsletterBuilder.ACL;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Web.Apps.NewsletterBuilder.Entities;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Controllers
{
    public class NewsletterTemplateController : MrCMSAppAdminController<NewsletterBuilderApp>
    {
        private readonly INewsletterTemplateAdminService _newsletterTemplateAdminService;

        public NewsletterTemplateController(INewsletterTemplateAdminService newsletterTemplateAdminService)
        {
            _newsletterTemplateAdminService = newsletterTemplateAdminService;
        }

        [MrCMSACLRule(typeof(NewsletterTemplateACL), NewsletterTemplateACL.List)]
        public ViewResult Index()
        {
            var newsletterTemplates = _newsletterTemplateAdminService.GetAll();
            return View(newsletterTemplates);
        }

        [HttpGet, MrCMSACLRule(typeof(NewsletterTemplateACL), NewsletterTemplateACL.Add)]
        public ViewResult Add()
        {
            return View();
        }

        [HttpPost, MrCMSACLRule(typeof(NewsletterTemplateACL), NewsletterTemplateACL.Add)]
        public RedirectToRouteResult Add(NewsletterTemplate newsletterTemplate)
        {
            _newsletterTemplateAdminService.Add(newsletterTemplate);
            TempData.SuccessMessages().Add("Template Added");
            return RedirectToAction("Edit", new{id = newsletterTemplate.Id});
        }

        [HttpGet, MrCMSACLRule(typeof(NewsletterTemplateACL), NewsletterTemplateACL.Edit)]
        public ViewResult Edit(NewsletterTemplate newsletterTemplate)
        {
            ViewData["template-data-options"] =_newsletterTemplateAdminService.GetTemplateDataOptions();
            return View(newsletterTemplate);
        }

        [HttpPost, ActionName("Edit"), MrCMSACLRule(typeof(NewsletterTemplateACL), NewsletterTemplateACL.Edit)]
        public RedirectToRouteResult Edit_POST(NewsletterTemplate newsletterTemplate)
        {
            _newsletterTemplateAdminService.Edit(newsletterTemplate);
            TempData.SuccessMessages().Add("Template Edited");
            return RedirectToAction("Edit", new { id = newsletterTemplate.Id });
        }

        [HttpGet, MrCMSACLRule(typeof(NewsletterTemplateACL), NewsletterTemplateACL.Delete)]
        public ViewResult Delete(NewsletterTemplate newsletterTemplate)
        {
            return View(newsletterTemplate);
        }

        [HttpPost, ActionName("Delete"), MrCMSACLRule(typeof(NewsletterTemplateACL), NewsletterTemplateACL.Delete)]
        public RedirectToRouteResult Delete_POST(NewsletterTemplate newsletterTemplate)
        {
            _newsletterTemplateAdminService.Delete(newsletterTemplate);

            return RedirectToAction("Index");
        }
    }
}