using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website.Controllers;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class PriceBreakController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductService _productService;

        public PriceBreakController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public PartialViewResult Add(int id, string type)
        {
            return PartialView(new AddPriceBreakModel {Id = id, Type = type});
        }

        [HttpPost]
        public RedirectResult Add(AddPriceBreakModel model)
        {
            var priceBreak = _productService.AddPriceBreak(model);

            return Redirect(priceBreak.Item.EditUrl);
        }

        public JsonResult IsQuantityValid(int quantity, int id, string type)
        {
            return Json(_productService.IsPriceBreakQuantityValid(quantity, id, type), JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsPriceValid(decimal price, int id, string type, int quantity)
        {
            return Json(_productService.IsPriceBreakPriceValid(price, id, type,quantity), JsonRequestBehavior.AllowGet);
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
            _productService.DeletePriceBreak(priceBreak);

            return Redirect(priceBreak.Item.EditUrl);
        }
    }
}