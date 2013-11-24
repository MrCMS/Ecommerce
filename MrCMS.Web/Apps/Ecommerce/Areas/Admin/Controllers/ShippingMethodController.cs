using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ShippingMethodController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IShippingMethodManager _shippingMethodManager;
        private readonly ITaxRateManager _taxRateManager;

        public ShippingMethodController(IShippingMethodManager shippingMethodManager, ITaxRateManager taxRateManager)
        {
            _shippingMethodManager = shippingMethodManager;
            _taxRateManager = taxRateManager;
        }

        public ViewResult Index()
        {
            var options = _shippingMethodManager.GetAll();
            return View(options);
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            ViewData["tax-rates"] = _taxRateManager.GetOptions();
            return PartialView();
        }

        [HttpPost]
        public RedirectToRouteResult Add(ShippingMethod option)
        {
            _shippingMethodManager.Add(option);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Edit(ShippingMethod option)
        {
            ViewData["tax-rates"] = _taxRateManager.GetOptions(option.TaxRate);
            return PartialView(option);
        }

        [ActionName("Edit")]
        [HttpPost]
        public RedirectToRouteResult Edit_POST(ShippingMethod option)
        {
            _shippingMethodManager.Update(option);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Delete(ShippingMethod option)
        {
            return PartialView(option);
        }

        [ActionName("Delete")]
        [HttpPost]
        public RedirectToRouteResult Delete_POST(ShippingMethod option)
        {
            _shippingMethodManager.Delete(option);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Sort()
        {
            var sortItems = _shippingMethodManager.GetAll().OrderBy(x => x.DisplayOrder)
                            .Select(
                                arg => new SortItem { Order = arg.DisplayOrder, Id = arg.Id, Name = arg.Name })
                            .ToList();
            return View(sortItems);
        }

        [HttpPost]
        public ActionResult Sort(List<SortItem> items)
        {
            if (items != null && items.Count > 0)
            {
                _shippingMethodManager.UpdateDisplayOrder(items);
            }
            return RedirectToAction("Index");
        }
    }
}