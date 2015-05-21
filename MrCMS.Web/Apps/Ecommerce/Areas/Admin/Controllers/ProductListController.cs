using System.Web.Mvc;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.ActionFilters;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductListController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IContentItemAdminService _contentItemAdminService;

        public ProductListController(IContentItemAdminService contentItemAdminService)
        {
            _contentItemAdminService = contentItemAdminService;
        }

        [HttpGet]
        [HandleMissingNewsletter]
        public ViewResult Add(int newsletterId = 0)
        {
            return View(_contentItemAdminService.GetNew<ProductList>(newsletterId));
        }

        [HttpPost]
        [AddSuccessMessage("Product List successfully added")]
        public RedirectToRouteResult Add(ProductList productList)
        {
            _contentItemAdminService.Add(productList);
            return RedirectToAction("Edit", "Newsletter", new { id = productList.Newsletter.Id });
        }

        public ViewResult Edit(ProductList productList)
        {
            return View(productList);
        }

        [HttpPost]
        [ActionName("Edit")]
        [AddSuccessMessage("Product List successfully updated")]
        public RedirectToRouteResult Edit_POST(ProductList productList)
        {
            _contentItemAdminService.Update(productList);
            return RedirectToAction("Edit", "Newsletter", new { id = productList.Newsletter.Id });
        }
    }
}