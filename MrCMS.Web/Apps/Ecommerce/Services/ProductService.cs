using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using MrCMS.Helpers;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly ISession _session;
        private readonly IDocumentService _documentService;

        public ProductService(ISession session, IDocumentService documentService)
        {
            _session = session;
            _documentService = documentService;
        }

        public ProductPagedList Search(string queryTerm = null, int page = 1)
        {
            IPagedList<Product> pagedList;
            if (!string.IsNullOrWhiteSpace(queryTerm))
            {
                pagedList =
                    _session.Paged(
                        QueryOver.Of<Product>()
                                 .Where(product => product.Name.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)), page,
                        10);
            }
            else
            {
                pagedList = _session.Paged(QueryOver.Of<Product>(), page, 10);
            }

            var productContainer = _documentService.GetUniquePage<ProductContainer>();
            var productContainerId = productContainer == null ? (int?)null : productContainer.Id;
            return new ProductPagedList(pagedList, productContainerId);
        }
    }
}