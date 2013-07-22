using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Website.Controllers;
using System;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductVariantController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductVariantService _productVariantService;
        private readonly ITaxRateManager _taxRateManager;
        private readonly ITrackingPolicyService _trackingPolicyService;

        public ProductVariantController(IProductVariantService productVariantService, ITaxRateManager taxRateManager, ITrackingPolicyService trackingPolicyService)
        {
            _productVariantService = productVariantService;
            _taxRateManager = taxRateManager;
            _trackingPolicyService = trackingPolicyService;
        }

        [HttpGet]
        public PartialViewResult Add(Product product)
        {
            ViewData["tax-rate-options"] = _taxRateManager.GetOptions();
            ViewData["tracking-policy"] = _trackingPolicyService.GetOptions();
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
        public ActionResult Add_POST(ProductVariant productVariant)
        {
            if (ModelState.IsValid)
            {
                _productVariantService.Add(productVariant);
                return RedirectToAction("Edit", "Webpage", new { id = productVariant.Product.Id });
            }
            ViewData["tax-rate-options"] = _taxRateManager.GetOptions(productVariant.TaxRate);
            ViewData["tracking-policy"] = _trackingPolicyService.GetOptions();
            return PartialView(productVariant);
        }

        [HttpGet]
        public PartialViewResult Edit(ProductVariant productVariant)
        {
            ViewData["tax-rate-options"] = _taxRateManager.GetOptions(productVariant.TaxRate);
            ViewData["tracking-policy"] = _trackingPolicyService.GetOptions();
            return PartialView(productVariant);
        }

        [ActionName("Edit")]
        [HttpPost]
        public ActionResult Edit_POST(ProductVariant productVariant)
        {
            if (ModelState.IsValid)
            {
                _productVariantService.Update(productVariant);
                return RedirectToAction("Edit", "Webpage", new { id = productVariant.Product.Id });
            }
            ViewData["tax-rate-options"] = _taxRateManager.GetOptions(productVariant.TaxRate);
            ViewData["tracking-policy"] = _trackingPolicyService.GetOptions();
            return PartialView(productVariant);
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

        public JsonResult IsUniqueSKU(string sku, int Id=0)
        {
            return _productVariantService.AnyExistingProductVariantWithSKU(sku, Id)
                           ? Json("There is already an SKU stored with that value.", JsonRequestBehavior.AllowGet)
                           : Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}