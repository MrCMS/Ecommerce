using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;
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
    }
}