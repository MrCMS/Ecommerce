using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using NHibernate;
using MrCMS.Helpers;
using System.Linq;
namespace MrCMS.Web.Apps.Ecommerce.Services.Tax
{
    public class TaxRateManager : ITaxRateManager
    {
        private readonly ISession _session;

        public TaxRateManager(ISession session)
        {
            _session = session;
        }
        public TaxRate Get(int id)
        {
            return _session.QueryOver<TaxRate>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }
        public TaxRate GetDefaultRate()
        {
            return _session.QueryOver<TaxRate>().Where(x => x.IsDefault==true).Cacheable().SingleOrDefault();
        }
        public IList<TaxRate> GetAll()
        {
            return _session.QueryOver<TaxRate>().Cacheable().List();
        }

        public void Add(TaxRate taxRate)
        {
            if (!GetAll().Any())
                taxRate.IsDefault = true;
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

        public void SetAllDefaultToFalse()
        {
            var taxes = GetAll();
            foreach (var taxRate in taxes)
            {
                taxRate.IsDefault = false;
                Update(taxRate);
            }
        }
    }
}