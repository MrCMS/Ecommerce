using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportTaxRates : IImportTaxRates
    {
        private readonly ISession _session;

        public ImportTaxRates(ISession session)
        {
            _session = session;
        }

        public string ProcessTaxRates(NopCommerceDataReader dataReader, NopImportContext nopImportContext)
        {
            HashSet<TaxData> taxDatas = dataReader.GetTaxData();

            _session.Transact(session =>
            {
                foreach (TaxData taxData in taxDatas)
                {
                    var taxRate = new TaxRate
                    {
                        Name = taxData.Name,
                        Percentage = taxData.Rate,
                    };
                    session.Save(taxRate);
                    nopImportContext.AddEntry(taxData.Id, taxRate);
                }
            });

            return string.Format("{0} tax rates processed", taxDatas.Count);

        }
    }
}