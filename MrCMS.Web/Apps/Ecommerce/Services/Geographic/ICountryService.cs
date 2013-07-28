using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;

namespace MrCMS.Web.Apps.Ecommerce.Services.Geographic
{
    public interface ICountryService
    {
        IList<Country> GetAllCountries();
        Country Get(int countryId);
        List<SelectListItem> GetCountriesToAdd();
        void AddCountry(Country country);
        void Save(Country country);
        void Delete(Country country);
        List<SelectListItem> GetOptions();
        bool AnyExistingCountriesWithName(string name, int id = 0);
        void UpdateDisplayOrder(IList<SortItem> options);
        Country GetCountryByCode(string code);
    }
}