using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Areas.Admin.Services.NopImport.Models;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public class ImportCountryData : IImportCountryData
    {
        private readonly ISession _session;

        public ImportCountryData(ISession session)
        {
            _session = session;
        }

        public string ProcessCountries(INopCommerceProductReader nopCommerceProductReader, string connectionString,
            NopImportContext nopImportContext)
        {
            List<CountryData> countryDatas = nopCommerceProductReader.GetCountryData(connectionString);
            _session.Transact(session =>
            {
                foreach (CountryData countryData in countryDatas)
                {
                    var country = new Country
                    {
                        Name = countryData.Name,
                        ISOTwoLetterCode = countryData.IsoCode
                    };
                    session.Save(country);
                    nopImportContext.AddEntry(countryData.Id, country);
                }
            });

            return string.Format("{0} countries processed", countryDatas.Count);
        }
    }
}