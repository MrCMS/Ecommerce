using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class BrandListingUIService : IBrandListingUIService
    {
        private readonly ISession _session;

        public BrandListingUIService(ISession session)
        {
            _session = session;
        }

        public IPagedList<Brand> GetBrands(int pageNumber, int pageSize)
        {
            return _session.QueryOver<Brand>().Where(x => x.Published).OrderBy(x => x.Name).Asc.Paged(pageNumber, pageSize);
        }
    }
}