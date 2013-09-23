using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services.Geographic
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
            return _session.QueryOver<Country>().OrderBy(x=>x.DisplayOrder).Asc.CacheMode(CacheMode.Put).List();
        }

        public Country Get(int id)
        {
            return _session.QueryOver<Country>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }

        public bool AnyExistingCountriesWithName(string name, int id=0)
        {
            if (id != 0 && Get(id).Name.Trim().ToLower() == name.Trim().ToLower())
            {
                return false;
            }
            return _session.QueryOver<Country>().Where(x => x.Name.IsInsensitiveLike(name, MatchMode.Exact)).RowCount() > 0;
        }

        public List<SelectListItem> GetCountriesToAdd()
        {
            return
                CountryIsoCodeAndNames.Where(
                    pair => !GetAllCountries().Select(country => country.ISOTwoLetterCode).Contains(pair.Key))
                                      .BuildSelectItemList(pair => pair.Value, pair => pair.Key, emptyItemText: null);
        }

        public void AddCountry(Country country)
        {
            //_session.Transact(
            //    session =>
            //    session.Save(new Country {ISOTwoLetterCode = countryCode, Name = CountryIsoCodeAndNames[countryCode]}));
            _session.Transact(
               session =>
               session.Save(country));
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
                var rows = CountryData.RawData.Split(new[] { "\n","\r" }, StringSplitOptions.RemoveEmptyEntries);
                return _countryIsoCodeAndNames ??
                       (_countryIsoCodeAndNames = rows.Select(s => s.Split(';')).ToDictionary(s => s[1], s => s[0]));
            }
        }

        public List<SelectListItem> GetOptions()
        {
            return GetAllCountries().BuildSelectItemList(country => country.Name, rate => rate.Id.ToString(),null, String.Empty);
        }

        public void UpdateDisplayOrder(IList<SortItem> options)
        {
            _session.Transact(session => options.ForEach(item =>
            {
                var formItem = session.Get<Country>(item.Id);
                formItem.DisplayOrder = item.Order;
                session.Update(formItem);
            }));
        }

        public Country GetCountryByCode(string code)
        {
            return _session.QueryOver<Country>()
                           .Where(country => country.ISOTwoLetterCode.IsInsensitiveLike(code, MatchMode.Exact))
                           .Take(1)
                           .Cacheable()
                           .SingleOrDefault();
        }
    }
}