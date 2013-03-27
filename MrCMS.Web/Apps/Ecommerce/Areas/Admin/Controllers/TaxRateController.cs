using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website.Controllers;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class TaxRateController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ITaxRateManager _taxRateManager;
        private readonly ICountryService _countryService;
        private readonly IRegionService _regionService;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly TaxSettings _taxSettings;

        public TaxRateController(ITaxRateManager taxRateManager, ICountryService countryService, IRegionService regionService, IConfigurationProvider configurationProvider, TaxSettings taxSettings)
        {
            _taxRateManager = taxRateManager;
            _countryService = countryService;
            _regionService = regionService;
            _configurationProvider = configurationProvider;
            _taxSettings = taxSettings;
        }

        public ViewResult Index()
        {
            ViewData["settings"] = _taxSettings;
            return View(_countryService.GetAllCountries());
        }

        [HttpGet]
        public ActionResult Add(int? countryId, int? regionId)
        {
            if (countryId.HasValue)
            {
                TaxRate tr = new TaxRate { Country = _countryService.Get(countryId.Value) };
                return PartialView(tr);
            }
            else if (regionId.HasValue)
            {
                Region region = _regionService.Get(regionId.Value);
                TaxRate tr = new TaxRate { Country = region.Country, Region = region };
                return PartialView(tr);
            }
            return RedirectToAction("Index");
        }

        [ActionName("Add")]
        [HttpPost]
        public RedirectToRouteResult Add_POST(TaxRate taxRate)
        {
            if (taxRate.Country != null || taxRate.Region != null)
            {
                _taxRateManager.Add(taxRate);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Edit(int? countryId, int? regionId)
        {
            TaxRate taxRate = null;
            if (countryId.HasValue)
            {
                taxRate = _taxRateManager.GetByCountryId(countryId.Value);
            }
            else if (regionId.HasValue)
            {
                taxRate = _taxRateManager.GetByRegionId(regionId.Value);
            }
            return PartialView(taxRate);
        }

        [ActionName("Edit")]
        [HttpPost]
        public RedirectToRouteResult Edit_POST(TaxRate taxRate)
        {
            if (taxRate.Country != null || taxRate.Region != null)
            {
                _taxRateManager.Update(taxRate);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Delete(int? countryId, int? regionId)
        {
            TaxRate taxRate = null;
            if (countryId.HasValue)
            {
                taxRate = _taxRateManager.GetByCountryId(countryId.Value);
            }
            else if (regionId.HasValue)
            {
                taxRate = _taxRateManager.GetByRegionId(regionId.Value);
            }
            return PartialView(taxRate);
        }

        [ActionName("Delete")]
        [HttpPost]
        public RedirectToRouteResult Delete_POST(TaxRate taxRate)
        {
            _taxRateManager.Delete(taxRate);
            return RedirectToAction("Index");
        }

        public RedirectToRouteResult Settings(TaxSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            return RedirectToAction("Index");
        }
    }
}