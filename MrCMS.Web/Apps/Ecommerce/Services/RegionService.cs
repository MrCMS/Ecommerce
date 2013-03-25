using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using NHibernate;

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
    }
}