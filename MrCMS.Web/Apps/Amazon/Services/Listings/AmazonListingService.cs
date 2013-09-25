using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Amazon.Services.Listings
{
    public class AmazonListingService : IAmazonListingService
    {
        private readonly ISession _session;
        private readonly IAmazonLogService _amazonLogService;

        public AmazonListingService(ISession session,
            IAmazonLogService amazonLogService)
        {
            _session = session;
            _amazonLogService = amazonLogService;
        }

        public AmazonListing Get(int id)
        {
            return _session.QueryOver<AmazonListing>()
                            .Where(item => item.Id == id).SingleOrDefault();
        }

        public AmazonListing GetByProductVariantSKU(string sku)
        {
            return _session.QueryOver<AmazonListing>()
                            .Where(item => item.SellerSKU.IsInsensitiveLike(sku,MatchMode.Exact)).SingleOrDefault();
        }

        public IPagedList<AmazonListing> Search(string queryTerm = null, int page = 1, int pageSize = 10)
        {
            if (!string.IsNullOrWhiteSpace(queryTerm))
            {
                return _session.QueryOver<AmazonListing>()
                                    .Where(x => 
                                        x.AmazonListingId.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)
                                        || x.SellerSKU.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)
                                        || x.ASIN.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)
                                        || x.Title.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)
                                        ).Paged(page, pageSize);
            }

            return _session.Paged(QueryOver.Of<AmazonListing>(), page, pageSize);
        }

        public void Save(AmazonListing item)
        {
            _session.Transact(session => session.SaveOrUpdate(item));
        }

        public void Delete(AmazonListing item)
        {
            _amazonLogService.Add(AmazonLogType.Listings, AmazonLogStatus.Delete, null, null, item);

            _session.Transact(session => session.Delete(item));
        }
    }
}