using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Currencies;
using MrCMS.Web.Apps.Ecommerce.Services.Currencies;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class CurrencyController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public ViewResult Index()
        {
            return View(_currencyService.GetAll());
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            return PartialView();
        }

        [HttpPost]
        public RedirectToRouteResult Add(Currency currency)
        {
            _currencyService.Add(currency);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        public PartialViewResult Edit_GET(Currency currency)
        {
            return PartialView(currency);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit(Currency currency)
        {
            _currencyService.Update(currency);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Delete")]
        public PartialViewResult Delete_GET(Currency currency)
        {
            return PartialView(currency);
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToRouteResult Delete(Currency currency)
        {
            _currencyService.Delete(currency);
            return RedirectToAction("Index");
        }
    }
}