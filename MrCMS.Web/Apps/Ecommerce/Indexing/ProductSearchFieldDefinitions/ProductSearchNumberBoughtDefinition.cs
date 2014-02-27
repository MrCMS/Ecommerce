using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchNumberBoughtDefinition : IntegerFieldDefinition<ProductSearchIndex, Product>
    {
        public ProductSearchNumberBoughtDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "numberbought", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<int> GetValues(Product obj)
        {
            yield return GetNumberBought(obj.Variants);
        }

        private static int GetNumberBought(IList<ProductVariant> variants)
        {
            var orderLines = MrCMSApplication.Get<ISession>().QueryOver<OrderLine>().Where(line => line.ProductVariant.IsIn(variants.ToList())).List();
            return orderLines.Sum(line => line.Quantity);
        }
    }
}