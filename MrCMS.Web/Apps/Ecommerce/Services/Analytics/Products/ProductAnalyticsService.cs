using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using NHibernate.Transform;

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
            var items = _session.CreateSQLQuery(@"SELECT TOP 20 P.Id,P.Name,P.UrlSegment,P.Abstract FROM Ecommerce_OrderLine AS OL 
                                                LEFT OUTER JOIN Ecommerce_ProductVariant AS PV ON OL.SKU=PV.SKU
                                                INNER JOIN Document AS P ON P.Id=PV.ProductId
                                                WHERE OL.OrderID IN (
                                                SELECT OrderID 
                                                FROM Ecommerce_OrderLine AS OL2 
                                                LEFT OUTER JOIN Ecommerce_ProductVariant AS PV2 ON OL2.SKU=PV2.SKU 
                                                WHERE PV2.ProductId=" +product.Id+@")
                                                AND PV.ProductId <> "+product.Id+@"
                                                GROUP BY P.Id,P.Name,P.UrlSegment,P.Abstract
                                                ORDER BY COUNT(OL.SKU) DESC")
                                                .SetResultTransformer(Transformers.AliasToBean<Product>()).List<Product>().ToList();

            return items;
        }
    }
}