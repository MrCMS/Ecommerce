using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Entities.Currencies;
using MrCMS.Web.Apps.Ecommerce.Services.Currencies;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class CurrencyController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [MrCMSACLRule(typeof(CurrencyACL), CurrencyACL.List)]
        public ViewResult Index()
        {
            return View(_currencyService.GetAll());
        }

        [HttpGet]
        [MrCMSACLRule(typeof(CurrencyACL), CurrencyACL.Add)]
        public PartialViewResult Add()
        {
            return PartialView();
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(CurrencyACL), CurrencyACL.Add)]
        public RedirectToRouteResult Add(Currency currency)
        {
            _currencyService.Add(currency);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        [MrCMSACLRule(typeof(CurrencyACL), CurrencyACL.Edit)]
        public PartialViewResult Edit_GET(Currency currency)
        {
            return PartialView(currency);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(CurrencyACL), CurrencyACL.Edit)]
        public RedirectToRouteResult Edit(Currency currency)
        {
            _currencyService.Update(currency);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Delete")]
        [MrCMSACLRule(typeof(CurrencyACL), CurrencyACL.Delete)]
        public PartialViewResult Delete_GET(Currency currency)
        {
            return PartialView(currency);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(CurrencyACL), CurrencyACL.Delete)]
        public RedirectToRouteResult Delete(Currency currency)
        {
            _currencyService.Delete(currency);
            return RedirectToAction("Index");
        }
    }
}