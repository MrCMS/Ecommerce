using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetCountryOptions : IGetCountryOptions
    {
        private readonly ISession _session;

        public GetCountryOptions(ISession session)
        {
            _session = session;
        }

        public List<SelectListItem> Get()
        {
            var countries =
                _session.QueryOver<Country>().OrderBy(country => country.DisplayOrder).Asc.Cacheable().List();

            if (countries.Count == 1)
                return countries.BuildSelectItemList(country => country.Name, country => country.ISOTwoLetterCode,
                    emptyItem: null);
            return countries.BuildSelectItemList(country => country.Name, country => country.ISOTwoLetterCode,
                emptyItem: null);
        }
    }
}