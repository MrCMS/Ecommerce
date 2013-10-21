using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace MrCMS.Web.Apps.Ecommerce.Services.Analytics.Products
{
    public interface IProductAnalyticsService
    {
        List<Product> GetListOfProductsWhoWhereAlsoBought(Product product);
    }

    public class ProductAnalyticsService : IProductAnalyticsService
    {
        private readonly ISession _session;

        public ProductAnalyticsService(ISession session)
        {
            _session = session;
        }

        public List<Product> GetListOfProductsWhoWhereAlsoBought(Product product)
        {
            var items = _session.CreateCriteria<Product>()
               .Add(
                          Subqueries.PropertyIn("Id",
                          DetachedCriteria.For<ProductVariant>()
                          .SetProjection(Projections.Property("Product.Id"))
                          .Add(Subqueries.PropertyIn("Id",
                              DetachedCriteria.For<OrderLine>()
                              .SetProjection(Projections.Property("ProductVariant.Id"))
                              .Add(Subqueries.PropertyIn("Order.Id",
                                  DetachedCriteria.For<OrderLine>()
                                  .SetProjection(Projections.Property("Order.Id"))
                                  .CreateCriteria("ProductVariant", JoinType.LeftOuterJoin)
                                  .Add(Restrictions.Eq("Product.Id", product.Id))
                              ))
                          ))
               ))
               .Add(Restrictions.Not(Restrictions.Eq("Id", product.Id))).SetCacheable(true)
               .SetMaxResults(20)
              .List<Product>().ToList();

            return items;
        }
    }
}