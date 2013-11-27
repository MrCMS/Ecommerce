using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductVariantController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductVariantService _productVariantService;
        private readonly ITaxRateManager _taxRateManager;
        private readonly IOptionService _optionService;
        private readonly IShippingMethodManager _shippingMethodManager;

        public ProductVariantController(IProductVariantService productVariantService, ITaxRateManager taxRateManager,
            IOptionService optionService, IShippingMethodManager shippingMethodManager)
        {
            _productVariantService = productVariantService;
            _taxRateManager = taxRateManager;
            _optionService = optionService;
            _shippingMethodManager = shippingMethodManager;
        }

        [HttpGet]
        public PartialViewResult Add(Product product)
        {
            ViewData["shipping-methods"] = _shippingMethodManager.GetAll();
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
        public ActionResult Add_POST(ProductVariant productVariant, string shippingMethodsValue="")
        {
            if (ModelState.IsValid)
            {
                SetShippingMethods(ref productVariant, shippingMethodsValue);
                _productVariantService.Add(productVariant);
                return RedirectToAction("Edit", "Webpage", new { id = productVariant.Product.Id });
            }
            ViewData["shipping-methods"] = _shippingMethodManager.GetAll();
            ViewData["tax-rate-options"] = _taxRateManager.GetOptions(productVariant.TaxRate);
            ViewData["tracking-policy"] = _optionService.GetEnumOptions<TrackingPolicy>();
            return PartialView(productVariant);
        }

        [HttpGet]
        public PartialViewResult Edit(ProductVariant productVariant)
        {
            ViewData["shipping-methods"] = _shippingMethodManager.GetAll();
            ViewData["tax-rate-options"] = _taxRateManager.GetOptions(productVariant.TaxRate);
            ViewData["tracking-policy"] = _optionService.GetEnumOptions<TrackingPolicy>();
            return PartialView(productVariant);
        }

        [ActionName("Edit")]
        [HttpPost]
        public ActionResult Edit_POST(ProductVariant productVariant, string shippingMethodsValue = "")
        {
            if (ModelState.IsValid)
            {
                SetShippingMethods(ref productVariant, shippingMethodsValue);
                _productVariantService.Update(productVariant);
                return RedirectToAction("Edit", "Webpage", new { id = productVariant.Product.Id });
            }
            ViewData["shipping-methods"] = _shippingMethodManager.GetAll();
            ViewData["tax-rate-options"] = _taxRateManager.GetOptions(productVariant.TaxRate);
            ViewData["tracking-policy"] = _optionService.GetEnumOptions<TrackingPolicy>();
            return PartialView(productVariant);
        }

        private void SetShippingMethods(ref ProductVariant productVariant, string shippingMethodsValue)
        {
            productVariant.ShippingMethods.Clear();

            if (string.IsNullOrWhiteSpace(shippingMethodsValue)) return;

            try
            {
                var smRaw = shippingMethodsValue.Trim().Split(',').Where(x => !string.IsNullOrWhiteSpace(x));
                foreach (var s in smRaw)
                {
                    int id;
                    Int32.TryParse(s, out id);
                    if (id <= 0) continue;
                    var shippingMethod = _shippingMethodManager.Get(id);
                    productVariant.ShippingMethods.Add(shippingMethod);
                }
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
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

        public JsonResult IsUniqueSKU(string sku, int Id = 0)
        {
            return _productVariantService.AnyExistingProductVariantWithSKU(sku, Id)
                       ? Json("There is already an SKU stored with that value.", JsonRequestBehavior.AllowGet)
                       : Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}