using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using MrCMS.Website.Controllers;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class CountryController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ICountryService _countryService;

        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        public ViewResult Index()
        {
            var countries = _countryService.GetAllCountries();
            return View(countries);
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            var countriesToAdd = _countryService.GetCountriesToAdd();
            return PartialView(countriesToAdd);
        }

        [HttpPost]
        [ActionName("Add")]
        public RedirectToRouteResult Add_POST(string countryCode)
        {
            _countryService.AddCountry(countryCode);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Edit(Country country)
        {
            return PartialView(country);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(Country country)
        {
            _countryService.Save(country);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Delete(Country country)
        {
            return PartialView(country);
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToRouteResult Delete_POST(Country country)
        {
            _countryService.Delete(country);

            return RedirectToAction("Index");
        }

       
    }
}