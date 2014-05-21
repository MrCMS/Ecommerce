using System.Linq;
using System.Web.Mvc;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using MrCMS.Website.Controllers;
using System.Collections.Generic;
using MrCMS.Website.Filters;

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
            //ADDING COUNTRIES FROM STATIC LIST
            //var countriesToAdd = _countryService.GetCountriesToAdd();
            //return PartialView(countriesToAdd);
            return PartialView();
        }

        [HttpPost]
        [ActionName("Add")]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Add_POST(Country country)
        {
            _countryService.AddCountry(country);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Edit(Country country)
        {
            return PartialView(country);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ForceImmediateLuceneUpdate]
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
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Delete_POST(Country country)
        {
            _countryService.Delete(country);

            return RedirectToAction("Index");
        }

        public JsonResult IsUniqueCountry(string name, int Id=0)
        {
            if (_countryService.AnyExistingCountriesWithName(name, Id))
                return Json("There is already a country stored with that name.", JsonRequestBehavior.AllowGet);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Sort()
        {
            var sortItems = _countryService.GetAllCountries().OrderBy(x => x.DisplayOrder)
                            .Select(
                                arg => new SortItem { Order = arg.DisplayOrder, Id = arg.Id, Name = arg.Name })
                            .ToList();
            return View(sortItems);
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public ActionResult Sort(List<SortItem> items)
        {
            if (items != null && items.Count > 0)
            {
                _countryService.UpdateDisplayOrder(items);
            }
            return RedirectToAction("Index");
        }
    }
}