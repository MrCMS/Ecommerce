using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Amazon.Services.Orders
{
    public class AmazonOrderService : IAmazonOrderService
    {
        private readonly ISession _session;

        public AmazonOrderService(ISession session)
        {
            _session = session;
        }

        public AmazonOrder Get(int id)
        {
            return _session.QueryOver<AmazonOrder>()
                            .Where(item => item.Id == id).SingleOrDefault();
        }

        public AmazonOrder GetByAmazonOrderId(string id)
        {
            return _session.QueryOver<AmazonOrder>()
                            .Where(item => item.AmazonOrderId.IsInsensitiveLike(id,MatchMode.Exact)).SingleOrDefault();
        }


        public IEnumerable<AmazonOrder> GetAll()
        {
            return _session.QueryOver<AmazonOrder>().Cacheable().List();
        }

        public IPagedList<AmazonOrder> Search(string queryTerm = null, int page = 1, int pageSize = 10)
        {
            if (!string.IsNullOrWhiteSpace(queryTerm))
            {
                return _session.QueryOver<AmazonOrder>()
                                    .Where(x => 
                                        x.BuyerEmail.IsInsensitiveLike(queryTerm, MatchMode.Anywhere) 
                                        || x.BuyerName.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)
                                        || x.AmazonOrderId.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)
                                        ).Paged(page, pageSize);
            }

            return _session.Paged(QueryOver.Of<AmazonOrder>(), page, pageSize);
        }

        public void Save(AmazonOrder item)
        {
            _session.Transact(session => session.SaveOrUpdate(item));
        }

        public void Delete(AmazonOrder item)
        {
            _session.Transact(session => session.Delete(item));
        }
    }
}