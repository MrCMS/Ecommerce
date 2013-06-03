using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Website.Controllers;
using System.Collections.Generic;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductVariantController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductVariantService _productVariantService;
        private readonly ITaxRateManager _taxRateManager;

        public ProductVariantController(IProductVariantService productVariantService, ITaxRateManager taxRateManager)
        {
            _productVariantService = productVariantService;
            _taxRateManager = taxRateManager;
        }

        [HttpGet]
        public PartialViewResult Add(Product product)
        {
            ViewData["taxrates"] = _taxRateManager.GetOptions();
            return
                PartialView(new ProductVariant
                                {
                                    Product = product,
                                    AttributeValues =
                                        Enumerable.Range(0, product.AttributeOptions.Count)
                                                  .Select(i => new ProductAttributeValue()).ToList()
                                });
        }

        [ActionName("Add")]
        [HttpPost]
        public RedirectToRouteResult Add_POST(ProductVariant productVariant)
        {
            _productVariantService.Add(productVariant);
            return RedirectToAction("Edit", "Webpage", new {id = productVariant.Product.Id});
        }

        [HttpGet]
        public PartialViewResult Edit(ProductVariant productVariant)
        {
            ViewData["taxrates"] = _taxRateManager.GetOptions(productVariant.TaxRate);
            return PartialView(productVariant);
        }

        [ActionName("Edit")]
        [HttpPost]
        public RedirectToRouteResult Edit_POST(ProductVariant productVariant)
        {
            _productVariantService.Update(productVariant);
            return RedirectToAction("Edit", "Webpage", new { id = productVariant.Product.Id });
        }

        [HttpGet]
        public PartialViewResult Delete(ProductVariant productVariant)
        {
            return PartialView(productVariant);
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToRouteResult Delete_POST(ProductVariant productVariant)
        {
            _productVariantService.Delete(productVariant);
            return RedirectToAction("Edit", "Webpage", new { id = productVariant.Product.Id });
        }
    }
}