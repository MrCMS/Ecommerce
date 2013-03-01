using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class TaxRateController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ITaxRateManager _taxRateManager;

        public TaxRateController(ITaxRateManager taxRateManager)
        {
            _taxRateManager = taxRateManager;
        }

        public ViewResult Index()
        {
            var taxRates = _taxRateManager.GetAll();
            return View(taxRates);
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            return PartialView(new TaxRate());
        }

        [ActionName("Add")]
        [HttpPost]
        public RedirectToRouteResult Add_POST(TaxRate taxRate)
        {
            _taxRateManager.Add(taxRate);
            return RedirectToAction("Edit", new { id = taxRate.Id });
        }

        [HttpGet]
        public ViewResult Edit(TaxRate taxRate)
        {
            return View(taxRate);
        }

        [ActionName("Edit")]
        [HttpPost]
        public RedirectToRouteResult Edit_POST(TaxRate taxRate)
        {
            _taxRateManager.Update(taxRate);
            return RedirectToAction("Edit", new { id = taxRate.Id });
        }

        [HttpGet]
        public PartialViewResult Delete(TaxRate taxRate)
        {
            return PartialView(taxRate);
        }

        [ActionName("Delete")]
        [HttpPost]
        public RedirectToRouteResult Delete_POST(TaxRate taxRate)
        {
            _taxRateManager.Delete(taxRate);
            return RedirectToAction("Index");
        }
    }
}