﻿using System.Collections.Generic;
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
        void AddCountry(string countryCode);
        void Save(Country country);
        void Delete(Country country);
        List<SelectListItem> GetOptions();
    }
}