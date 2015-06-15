using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.TemplateData;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.ActionFilters;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductListTemplateDataController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ITemplateDataAdminService _templateDataAdminService;

        public ProductListTemplateDataController(ITemplateDataAdminService templateDataAdminService)
        {
            _templateDataAdminService = templateDataAdminService;
        }

        [HttpGet]
        [HandleMissingTemplate]
        public ViewResult Add(int templateId = 0)
        {
            var productListTemplateData = _templateDataAdminService.GetNew<ProductListTemplateData>(templateId);
            return View(productListTemplateData);
        }

        [HttpPost]
        [AddSuccessMessage("Product List template data added")]
        public RedirectToRouteResult Add(ProductListTemplateData data)
        {
            _templateDataAdminService.Add(data);
            return RedirectToAction("Edit", "NewsletterTemplate", new { id = data.NewsletterTemplate.Id });
        }

        [HttpGet]
        public ViewResult Edit(ProductListTemplateData data)
        {
            return View(data);
        }

        [HttpPost]
        [ActionName("Edit")]
        [AddSuccessMessage("Product List template data updated")]
        public RedirectToRouteResult Edit_POST(ProductListTemplateData data)
        {
            _templateDataAdminService.Update(data);
            return RedirectToAction("Edit", "NewsletterTemplate", new { id = data.NewsletterTemplate.Id });
        }
    }
}