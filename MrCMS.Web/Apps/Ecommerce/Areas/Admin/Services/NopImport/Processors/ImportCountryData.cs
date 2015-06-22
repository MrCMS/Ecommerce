using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportCountryData : IImportCountryData
    {
        private readonly IStatelessSession _session;
        private readonly Site _site;

        public ImportCountryData(IStatelessSession session,Site site)
        {
            _session = session;
            _site = site;
        }

        public string ProcessCountries(NopCommerceDataReader dataReader, 
            NopImportContext nopImportContext)
        {
            var countryDatas = dataReader.GetCountryData();
            var site = _session.Get<Site>(_site.Id);
            _session.Transact(session =>
            {
                foreach (CountryData countryData in countryDatas)
                {
                    var country = new Country
                    {
                        Name = countryData.Name,
                        ISOTwoLetterCode = countryData.IsoCode
                    };
                    country.AssignBaseProperties(site);
                    session.Insert(country);
                    nopImportContext.AddEntry(countryData.Id, country);
                }
            });

            return string.Format("{0} countries processed", countryDatas.Count);
        }
    }
}