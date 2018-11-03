using System;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class SKUSearchService : ISKUSearchService
    {
        private readonly ISession _session;

        public SKUSearchService(ISession session)
        {
            _session = session;
        }
        public IPagedList<ProductVariant> Search(string query, string skus, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query))
                return PagedList<ProductVariant>.Empty;

            var skuList = (skus ?? string.Empty).Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).ToList();

            Product productAlias = null;
            var queryOver = _session.QueryOver<ProductVariant>()
                .JoinAlias(x => x.Product, () => productAlias)
                .Fetch(x => x.Product).Eager
                .Where(
                    x =>
                        x.Name.IsInsensitiveLike(query, MatchMode.Anywhere) ||
                        productAlias.Name.IsInsensitiveLike(query, MatchMode.Anywhere) ||
                        x.SKU.IsInsensitiveLike(query, MatchMode.Anywhere));

            if (skuList.Any())
                queryOver = queryOver.Where(x => !x.SKU.IsIn(skuList));

            return queryOver
                .OrderBy(() => productAlias.PublishOn).Desc
                .Paged(page);
        }
    }
}