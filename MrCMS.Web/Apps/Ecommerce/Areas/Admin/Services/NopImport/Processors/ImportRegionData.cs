using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportRegionData : IImportRegionData
    {
        private readonly IStatelessSession _session;
        private readonly Site _site;

        public ImportRegionData(IStatelessSession session,Site site)
        {
            _session = session;
            _site = site;
        }

        public string ProcessRegions(NopCommerceDataReader dataReader, NopImportContext nopImportContext)
        {

            HashSet<RegionData> regionDatas = dataReader.GetRegionData();
            var site = _session.Get<Site>(_site.Id);
            _session.Transact(session =>
            {
                foreach (RegionData regionData in regionDatas)
                {
                    var region = new Region
                    {
                        Name = regionData.Name,
                        Country = nopImportContext.FindNew<Country>(regionData.CountryId)
                    };
                    region.AssignBaseProperties(site);
                    session.Insert(region);
                    nopImportContext.AddEntry(regionData.Id, region);
                }
            });

            return string.Format("{0} regions processed", regionDatas.Count);

        }
    }
}