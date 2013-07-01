using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website.Controllers;

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
        public RedirectResult Add(AddPriceBreakModel model)
        {
            var priceBreak = _productVariantService.AddPriceBreak(model);

            return Redirect(priceBreak.Item.EditUrl);
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
        public RedirectResult Delete_POST(PriceBreak priceBreak)
        {
            _productVariantService.DeletePriceBreak(priceBreak);

            return Redirect(priceBreak.Item.EditUrl);
        }
    }
}