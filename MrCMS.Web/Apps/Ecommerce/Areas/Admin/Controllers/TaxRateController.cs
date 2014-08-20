using System;
using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using System.Linq;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class TaxRateController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ITaxRateManager _taxRateManager;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly TaxSettings _taxSettings;

        public TaxRateController(ITaxRateManager taxRateManager, IConfigurationProvider configurationProvider,TaxSettings taxSettings)
        {
            _taxRateManager = taxRateManager;
            _configurationProvider = configurationProvider;
            _taxSettings = taxSettings;
        }

        [MrCMSACLRule(typeof(TaxRateACL), TaxRateACL.List)]
        public ViewResult Index()
        {
            ViewData["settings"] = _taxSettings;
            var taxRates = _taxRateManager.GetAll().OrderByDescending(x=>x.IsDefault).ThenBy(x=>x.Percentage).ToList();
            return View(taxRates);
        }

        [HttpGet]
        [MrCMSACLRule(typeof(TaxRateACL), TaxRateACL.Add)]
        public PartialViewResult Add(string source="")
        {
            ViewBag.Source = source;
            return PartialView(new TaxRate());
        }

        [ActionName("Add")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(TaxRateACL), TaxRateACL.Add)]
        public RedirectToRouteResult Add_POST(TaxRate taxRate, string source="")
        {
            _taxRateManager.Add(taxRate);
            if (!String.IsNullOrWhiteSpace(source) && source == "settings")
            {
                var settings = _taxSettings;
                settings.TaxesEnabled = true;
                _configurationProvider.SaveSettings(_taxSettings);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Edit", new { id = taxRate.Id });
        }

        [HttpGet]
        [MrCMSACLRule(typeof(TaxRateACL), TaxRateACL.Edit)]
        public ViewResult Edit(TaxRate taxRate)
        {
            return View(taxRate);
        }

        [ActionName("Edit")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(TaxRateACL), TaxRateACL.Edit)]
        public RedirectToRouteResult Edit_POST(TaxRate taxRate)
        {
            _taxRateManager.Update(taxRate);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [MrCMSACLRule(typeof(TaxRateACL), TaxRateACL.Delete)]
        public PartialViewResult Delete(TaxRate taxRate)
        {
            return PartialView(taxRate);
        }

        [ActionName("Delete")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(TaxRateACL), TaxRateACL.Delete)]
        public RedirectToRouteResult Delete_POST(TaxRate taxRate)
        {
            _taxRateManager.Delete(taxRate);
            if (_taxRateManager.GetDefaultRate() == null)
            {
                var settings = _taxSettings;
                settings.TaxesEnabled = false;
                _configurationProvider.SaveSettings(_taxSettings);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(TaxRateACL), TaxRateACL.Settings)]
        public ActionResult Settings(TaxSettings settings)
        {
            if (settings.TaxesEnabled && _taxRateManager.GetDefaultRate() == null)
            {
                ViewBag.Status = "false";
                ViewData["settings"] = _taxSettings;
                var taxRates = _taxRateManager.GetAll().OrderByDescending(x => x.IsDefault).ThenBy(x => x.Percentage).ToList();
                return View("Index",taxRates);
            }
             _configurationProvider.SaveSettings(settings);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public RedirectToRouteResult MakeDefault(TaxRate taxRate)
        {
            _taxRateManager.SetAllDefaultToFalse();
            taxRate.IsDefault = true;
            _taxRateManager.Update(taxRate);
            return RedirectToAction("Index");
        }
    }
}