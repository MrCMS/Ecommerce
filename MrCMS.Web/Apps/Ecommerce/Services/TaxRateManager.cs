using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;
using NHibernate;
using MrCMS.Helpers;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class TaxRateManager : ITaxRateManager
    {
        private readonly ISession _session;

        public TaxRateManager(ISession session)
        {
            _session = session;
        }

        public IList<TaxRate> GetAll()
        {
            return _session.QueryOver<TaxRate>().OrderBy(x => x.Country).Asc.OrderBy(x => x.Region).Asc.CacheMode(CacheMode.Refresh).List();
        }

        public TaxRate GetByCountryId(int countryId)
        {
            return _session.QueryOver<TaxRate>().Where(x => x.Country.Id == countryId && x.Region==null).Cacheable().SingleOrDefault();
        }
        public TaxRate GetByRegionId(int regionId)
        {
            return _session.QueryOver<TaxRate>().Where(x => x.Region.Id == regionId).Cacheable().SingleOrDefault();
        }

        public void Add(TaxRate taxRate)
        {
            _session.Transact(session =>
                                  {
                                      if (taxRate.Country != null)
                                          taxRate.Country.TaxRates.Add(taxRate);
                                      if (taxRate.Region != null)
                                          taxRate.Region.TaxRates.Add(taxRate);
                                      session.Save(taxRate);
                                  });
        }

        public void Update(TaxRate taxRate)
        {
            _session.Transact(session => session.Update(taxRate));
        }

        public void Delete(TaxRate taxRate)
        {
            _session.Transact(session => session.Delete(taxRate));
        }

        public List<SelectListItem> GetOptions()
        {
            return GetAll().BuildSelectItemList(rate => rate.Name, rate => rate.Id.ToString(), emptyItemText: "None");
        }
    }
}