using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;
using MrCMS.Web.Apps.Ecommerce.Services.NewsletterBuilder;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class NewsletterTemplateController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly INewsletterTemplateService _newsletterTemplateService;

        public NewsletterTemplateController(INewsletterTemplateService newsletterTemplateService)
        {
            _newsletterTemplateService = newsletterTemplateService;
        }

        public ViewResult Index()
        {
            var newsletterTemplates = _newsletterTemplateService.GetAll();
            return View(newsletterTemplates);
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            return PartialView();
        }

        [HttpPost]
        public RedirectToRouteResult Add(NewsletterTemplate newsletterTemplate)
        {
            _newsletterTemplateService.Add(newsletterTemplate);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Edit(NewsletterTemplate newsletterTemplate)
        {
            return PartialView(newsletterTemplate);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(NewsletterTemplate newsletterTemplate)
        {
            _newsletterTemplateService.Edit(newsletterTemplate);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Delete(NewsletterTemplate newsletterTemplate)
        {
            return PartialView(newsletterTemplate);
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToRouteResult Delete_POST(NewsletterTemplate newsletterTemplate)
        {
            _newsletterTemplateService.Delete(newsletterTemplate);

            return RedirectToAction("Index");
        }
    }
}