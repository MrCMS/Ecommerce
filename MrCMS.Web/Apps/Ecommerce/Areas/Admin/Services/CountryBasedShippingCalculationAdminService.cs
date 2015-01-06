using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class CountryBasedShippingCalculationAdminService : ICountryBasedShippingCalculationAdminService
    {
        private readonly IGetShippingCriteriaOptions _getShippingCriteriaOptions;
        private readonly ISession _session;

        public CountryBasedShippingCalculationAdminService(ISession session,
            IGetShippingCriteriaOptions getShippingCriteriaOptions)
        {
            _session = session;
            _getShippingCriteriaOptions = getShippingCriteriaOptions;
        }

        public List<SelectListItem> GetCriteriaOptions()
        {
            return _getShippingCriteriaOptions.Get();
        }

        public List<SelectListItem> GetCountryOptions()
        {
            IList<Country> countries =
                _session.QueryOver<Country>().OrderBy(country => country.Name).Asc.Cacheable().List();

            return countries.BuildSelectItemList(country => country.Name, country => country.ISOTwoLetterCode,
                emptyItem: null);
        }

        public void Add(CountryBasedShippingCalculation countryBasedShippingCalculation)
        {
            _session.Transact(session => session.Save(countryBasedShippingCalculation));
        }

        public void Update(CountryBasedShippingCalculation countryBasedShippingCalculation)
        {
            _session.Transact(session => session.Update(countryBasedShippingCalculation));
        }
    }
}