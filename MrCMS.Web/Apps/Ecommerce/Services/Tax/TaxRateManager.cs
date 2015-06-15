using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Tax
{
    public class TaxRateManager : ITaxRateManager
    {
        private readonly IGetDefaultTaxRate _getDefaultTaxRate;
        private readonly IProductVariantService _productVariantService;
        private readonly ISession _session;

        public TaxRateManager(ISession session, IProductVariantService productVariantService,
            IGetDefaultTaxRate getDefaultTaxRate)
        {
            _session = session;
            _productVariantService = productVariantService;
            _getDefaultTaxRate = getDefaultTaxRate;
        }

        public TaxRate Get(int id)
        {
            return _session.Get<TaxRate>(id);
        }

        public TaxRate GetDefaultRate()
        {
            return _getDefaultTaxRate.Get();
        }

        public TaxRate GetRateForOrderLine(OrderLine orderLine)
        {
            TaxRate taxRate = null;
            ProductVariant pv = _productVariantService.GetProductVariantBySKU(orderLine.SKU);
            if (pv != null && pv.TaxRate != null)
                taxRate = pv.TaxRate;
            return taxRate ?? GetDefaultRate();
        }

        public IList<TaxRate> GetAll()
        {
            return _session.QueryOver<TaxRate>().Cacheable().List().OrderByDescending(x => x.IsDefault).ThenBy(x => x.Percentage).ToList();
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

        public void SetAllDefaultToFalse()
        {
            IList<TaxRate> taxes = GetAll();
            foreach (TaxRate taxRate in taxes)
            {
                taxRate.IsDefault = false;
                Update(taxRate);
            }
        }
    }
}