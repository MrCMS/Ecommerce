using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Areas.Admin.Services.NopImport.Models;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public class ImportRegionData : IImportRegionData
    {
        private readonly ISession _session;

        public ImportRegionData(ISession session)
        {
            _session = session;
        }

        public string ProcessRegions(INopCommerceProductReader nopCommerceProductReader, string connectionString,
            NopImportContext nopImportContext)
        {
            
            List<RegionData> regionDatas = nopCommerceProductReader.GetRegionData(connectionString);
            _session.Transact(session =>
            {
                foreach (RegionData regionData in regionDatas)
                {
                    var region = new Region
                    {
                        Name = regionData.Name,
                        Country = nopImportContext.FindNew<Country>(regionData.CountryId)
                    };
                    session.Save(region);
                    nopImportContext.AddEntry(regionData.Id, region);
                }
            });

            return string.Format("{0} regions processed", regionDatas.Count);

        }
    }
}