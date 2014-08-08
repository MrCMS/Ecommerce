using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Tax
{
    public class GetTaxRateOptions : IGetTaxRateOptions
    {
        private readonly ISession _session;

        public GetTaxRateOptions(ISession session)
        {
            _session = session;
        }

        public List<SelectListItem> GetOptions(TaxRate taxRate)
        {
            return GetOptions(taxRate == null ? (int?) null : taxRate.Id);
        }

        public List<SelectListItem> GetOptions(int? id = null)
        {
            return _session.QueryOver<TaxRate>().Cacheable().List().OrderBy(rate => rate.Name)
                .BuildSelectItemList(rate => rate.Name, rate => rate.Id.ToString(), rate => rate.Id == id,
                    "Default Tax Rate");
        }
    }
}