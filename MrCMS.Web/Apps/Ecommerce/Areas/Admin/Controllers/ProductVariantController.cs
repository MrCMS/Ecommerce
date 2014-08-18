using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductVariantController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IGetGiftCardTypeOptions _getGiftCardTypeOptions;
        private readonly IGetProductVariantTypeOptions _getProductVariantTypeOptions;
        private readonly IGetTaxRateOptions _getTaxRateOptions;
        private readonly IOptionService _optionService;
        private readonly IProductVariantService _productVariantService;

        public ProductVariantController(IProductVariantService productVariantService,
            IOptionService optionService, IGetTaxRateOptions getTaxRateOptions,
            IGetGiftCardTypeOptions getGiftCardTypeOptions,
            IGetProductVariantTypeOptions getProductVariantTypeOptions)
        {
            _productVariantService = productVariantService;
            _optionService = optionService;
            _getTaxRateOptions = getTaxRateOptions;
            _getGiftCardTypeOptions = getGiftCardTypeOptions;
            _getProductVariantTypeOptions = getProductVariantTypeOptions;
        }

        [HttpGet]
        public PartialViewResult Add(Product product)
        {
            ViewData["gift-card-type-options"] = _getGiftCardTypeOptions.Get();
            ViewData["product-variant-type-options"] = _getProductVariantTypeOptions.Get();
            ViewData["tax-rate-options"] = _getTaxRateOptions.GetOptions();
            ViewData["tracking-policy"] = _optionService.GetEnumOptions<TrackingPolicy>();
            var productVariant = new ProductVariant
            {
                Product = product,
                OptionValues = Enumerable.Range(0, product.Options.Count).Select(i => new ProductOptionValue()).ToList()
            };
            return
                PartialView(productVariant);
        }

        [ActionName("Add")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public ActionResult Add_POST([IoCModelBinder(typeof (ProductVariantModelBinder))] ProductVariant productVariant)
        {
            if (ModelState.IsValid)
            {
                _productVariantService.Add(productVariant);
                return RedirectToAction("Edit", "Webpage", new {id = productVariant.Product.Id});
            }
            return RedirectToAction("Add", "ProductVariant", new {id = productVariant.Product.Id});
        }

        [HttpGet]
        public PartialViewResult Edit(ProductVariant productVariant)
        {
            ModelState.Clear();
            ViewData["gift-card-type-options"] = _getGiftCardTypeOptions.Get();
            ViewData["product-variant-type-options"] = _getProductVariantTypeOptions.Get();
            ViewData["tax-rate-options"] = _getTaxRateOptions.GetOptions(productVariant.TaxRate);
            ViewData["tracking-policy"] = _optionService.GetEnumOptions<TrackingPolicy>();
            return PartialView(productVariant);
        }

        [ActionName("Edit")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public ActionResult Edit_POST([IoCModelBinder(typeof (ProductVariantModelBinder))] ProductVariant productVariant)
        {
            if (ModelState.IsValid)
            {
                _productVariantService.Update(productVariant);
                return RedirectToAction("Edit", "Webpage", new {id = productVariant.Product.Id});
            }
            return RedirectToAction("Edit", "ProductVariant", new {id = productVariant.Product.Id});
        }

        [HttpGet]
        public PartialViewResult Delete(ProductVariant productVariant)
        {
            return PartialView(productVariant);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Delete_POST(ProductVariant productVariant)
        {
            _productVariantService.Delete(productVariant);
            return RedirectToAction("Edit", "Webpage", new {id = productVariant.Product.Id});
        }

        public JsonResult IsUniqueSKU(string sku, int Id = 0)
        {
            return _productVariantService.AnyExistingProductVariantWithSKU(sku, Id)
                ? Json("There is already an SKU stored with that value.", JsonRequestBehavior.AllowGet)
                : Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}