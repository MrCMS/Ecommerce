using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using MrCMS.Helpers;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly ISession _session;

        public ProductService(ISession session)
        {
            _session = session;
        }

        public IPagedList<Product> Search(string queryTerm = null, int page = 1)
        {
            if (!string.IsNullOrWhiteSpace(queryTerm))
            {
                return
                    _session.Paged(
                        QueryOver.Of<Product>()
                                 .Where(product => product.Name.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)), page,
                        10);
            }
            return _session.Paged(QueryOver.Of<Product>(), page, 10);
        }
    }
}