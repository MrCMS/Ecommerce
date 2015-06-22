using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportTaxRates : IImportTaxRates
    {
        private readonly IStatelessSession _session;
        private readonly Site _site;

        public ImportTaxRates(IStatelessSession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public string ProcessTaxRates(NopCommerceDataReader dataReader, NopImportContext nopImportContext)
        {
            HashSet<TaxData> taxDatas = dataReader.GetTaxData();
            var site = _session.Get<Site>(_site.Id);
            _session.Transact(session =>
            {
                foreach (TaxData taxData in taxDatas)
                {
                    var taxRate = new TaxRate
                    {
                        Name = taxData.Name,
                        Percentage = taxData.Rate,
                    };
                    taxRate.AssignBaseProperties(site);
                    session.Insert(taxRate);
                    nopImportContext.AddEntry(taxData.Id, taxRate);
                }
            });

            return string.Format("{0} tax rates processed", taxDatas.Count);

        }
    }
}