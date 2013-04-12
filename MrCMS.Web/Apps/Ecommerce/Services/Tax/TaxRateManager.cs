using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Services.Tax
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
            return _session.QueryOver<TaxRate>().Cacheable().List();
        }

        public void Add(TaxRate taxRate)
        {
            _session.Transact(session => session.Save(taxRate));
        }

        public void Update(TaxRate taxRate)
        {
            _session.Transact(session => session.Update(taxRate));
        }

        public void Delete(TaxRate taxRate)
        {
            _session.Transact(session => session.Delete(taxRate));
        }

        public List<SelectListItem> GetOptions(TaxRate taxRate = null)
        {
            return GetAll()
                .BuildSelectItemList(rate => rate.Name, rate => rate.Id.ToString(), rate => rate == taxRate,
                                     emptyItem: null);
        }
    }
}