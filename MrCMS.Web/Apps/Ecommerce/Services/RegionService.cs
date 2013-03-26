using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class RegionService : IRegionService
    {
        private readonly ISession _session;

        public RegionService(ISession session)
        {
            _session = session;
        }

        public void Add(Region region)
        {
            _session.Transact(session =>
                                  {
                                      session.Save(region);
                                      if (region.Country != null)
                                          region.Country.Regions.Add(region);
                                  });
        }

        public void Update(Region region)
        {
            _session.Transact(session => session.Update(region));
        }

        public void Delete(Region region)
        {
            _session.Transact(session => session.Delete(region));
        }

        public object GetRegionsByCountryId(int countryId)
        {
            IList<Region> regions = _session.QueryOver<Region>().Cacheable().List();
            return (from r in regions where r.Country.Id == countryId select new { r.Id, r.Name });
        }

        public Region Get(int regionId)
        {
            return _session.QueryOver<Region>().Where(x => x.Id == regionId).Cacheable().SingleOrDefault();
        }
    }
}