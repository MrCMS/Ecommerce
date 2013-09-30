using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Amazon.Services.Listings
{
    public class AmazonListingGroupService : IAmazonListingGroupService
    {
        private readonly ISession _session;
        private readonly IAmazonLogService _amazonLogService;

        public AmazonListingGroupService(ISession session, IAmazonLogService amazonLogService)
        {
            _session = session;
            _amazonLogService = amazonLogService;
        }

        public AmazonListingGroup Get(int id)
        {
            return _session.QueryOver<AmazonListingGroup>()
                            .Where(item => item.Id == id).SingleOrDefault();
        }

        public IPagedList<AmazonListingGroup> Search(string queryTerm = null, int page = 1, int pageSize = 10)
        {
            if (!string.IsNullOrWhiteSpace(queryTerm))
            {
                return _session.QueryOver<AmazonListingGroup>()
                                    .Where(x =>x.Name.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)).Paged(page, pageSize);
            }

            return _session.Paged(QueryOver.Of<AmazonListingGroup>(), page, pageSize);
        }

        public void Save(AmazonListingGroup item)
        {
            var id = item.Id;
            _session.Transact(session => session.SaveOrUpdate(item));
            _amazonLogService.Add(AmazonLogType.ListingGroups, id > 0 ? AmazonLogStatus.Update : AmazonLogStatus.Insert,
                null, null, null, null, null, null, item);
        }

        public void Delete(AmazonListingGroup item)
        {
            _amazonLogService.Add(AmazonLogType.ListingGroups, AmazonLogStatus.Delete,null,null,null,null,null,null,item);

            _session.Transact(session => session.Delete(item));
        }
    }
}