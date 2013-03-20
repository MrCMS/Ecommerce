using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class CountryService : ICountryService
    {
        private readonly ISession _session;

        public CountryService(ISession session)
        {
            _session = session;
        }

        public IList<Country> GetAllCountries()
        {
            return _session.QueryOver<Country>().Cacheable().List();
        }

        public List<SelectListItem> GetCountriesToAdd()
        {
            return
                CountryIsoCodeAndNames.Where(
                    pair => !GetAllCountries().Select(country => country.ISOTwoLetterCode).Contains(pair.Key))
                                      .BuildSelectItemList(pair => pair.Value, pair => pair.Key, emptyItemText: null);
        }

        public void AddCountry(string countryCode)
        {
            _session.Transact(
                session =>
                session.Save(new Country {ISOTwoLetterCode = countryCode, Name = CountryIsoCodeAndNames[countryCode]}));
        }

        public void Save(Country country)
        {
            _session.Transact(session => session.SaveOrUpdate(country));
        }

        public void Delete(Country country)
        {
            _session.Transact(session => session.Delete(country));
        }

        private static Dictionary<string, string> _countryIsoCodeAndNames;
        private static Dictionary<string, string> CountryIsoCodeAndNames
        {
            get
            {
                var rows = CountryData.RawData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                return _countryIsoCodeAndNames ??
                       (_countryIsoCodeAndNames = rows.Select(s => s.Split(';')).ToDictionary(s => s[1], s => s[0]));
            }
        }
    }
}