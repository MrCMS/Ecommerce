using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Indexing;
using MrCMS.Indexing.Management;
using MrCMS.Tasks;
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
        private readonly ISession _session;

        public ProductSearchNumberBoughtDefinition(ISession session, ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "numberbought", index: Field.Index.NOT_ANALYZED)
        {
            _session = session;
        }

        protected override IEnumerable<int> GetValues(Product obj)
        {
            yield return GetNumberBought(obj.Variants);
        }

        private int GetNumberBought(IList<ProductVariant> variants)
        {
            var orderLines = _session.QueryOver<OrderLine>().Where(line => line.ProductVariant.IsIn(variants.ToList())).List();
            return orderLines.Sum(line => line.Quantity);
        }
        public override Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>> GetRelatedEntities()
        {
            return new Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>>
                       {
                           {
                               typeof (OrderLine),
                               GetActions
                           }
                       };
        }

        private static IEnumerable<LuceneAction> GetActions(SystemEntity entity)
        {
            var line = entity as OrderLine;
            if (line != null && line.ProductVariant != null && line.ProductVariant.Product != null)
                yield return new LuceneAction
                                 {
                                     Entity = line.ProductVariant.Product.Unproxy(),
                                     Operation = LuceneOperation.Update,
                                     IndexDefinition =
                                         IndexingHelper.Get<ProductSearchIndex>()
                                 };
        }
    }
}