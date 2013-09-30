using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductVariantController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductVariantService _productVariantService;
        private readonly ITaxRateManager _taxRateManager;
        private readonly IOptionService _optionService;

        public ProductVariantController(IProductVariantService productVariantService, ITaxRateManager taxRateManager,
            IOptionService optionService)
        {
            _productVariantService = productVariantService;
            _taxRateManager = taxRateManager;
            _optionService = optionService;
        }

        [HttpGet]
        public PartialViewResult Add(Product product)
        {
            ViewData["tax-rate-options"] = _taxRateManager.GetOptions();
            ViewData["tracking-policy"] = _optionService.GetEnumOptions<TrackingPolicy>();
            return
                PartialView(new ProductVariant
                                {
                                    Product = product,
                                    OptionValues =
                                        Enumerable.Range(0, product.Options.Count)
                                                  .Select(i => new ProductOptionValue()).ToList()
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
            ViewData["tracking-policy"] = _optionService.GetEnumOptions<TrackingPolicy>();
            return PartialView(productVariant);
        }

        [HttpGet]
        public PartialViewResult Edit(ProductVariant productVariant)
        {
            ViewData["tax-rate-options"] = _taxRateManager.GetOptions(productVariant.TaxRate);
            ViewData["tracking-policy"] = _optionService.GetEnumOptions<TrackingPolicy>();
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
            ViewData["tracking-policy"] = _optionService.GetEnumOptions<TrackingPolicy>();
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