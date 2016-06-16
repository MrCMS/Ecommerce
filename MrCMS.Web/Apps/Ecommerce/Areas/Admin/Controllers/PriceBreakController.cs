using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class PriceBreakController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductVariantService _productVariantService;

        public PriceBreakController(IProductVariantService productVariantService)
        {
            _productVariantService = productVariantService;
        }

        [HttpGet]
        public PartialViewResult Add(int id)
        {
            return PartialView(new AddPriceBreakModel { Id = id });
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Add(AddPriceBreakModel model)
        {
            var priceBreak = _productVariantService.AddPriceBreak(model);
            // product is edited by the webpage controller as it is a webpage
            return RedirectToAction("Edit", "Webpage", new { id = priceBreak.ProductVariant.Product.Id });
        }

        public JsonResult IsQuantityValid(int quantity, ProductVariant productVariant)
        {
            return Json(_productVariantService.IsPriceBreakQuantityValid(quantity, productVariant), JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsPriceValid(decimal price, ProductVariant productVariant, int quantity)
        {
            return Json(_productVariantService.IsPriceBreakPriceValid(price, productVariant, quantity), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult Delete(PriceBreak priceBreak)
        {
            return PartialView(priceBreak);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Delete_POST(PriceBreak priceBreak)
        {
            _productVariantService.DeletePriceBreak(priceBreak);
            // product is edited by the webpage controller as it is a webpage
            return RedirectToAction("Edit", "Webpage", new { id = priceBreak.ProductVariant.Product.Id });
        }
    }
}