using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
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
using NHibernate.Transform;

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
            var values = variants.Select(variant => variant.Id).ToList();
            var numberBoughtCount = new NumberBoughtCount();
            var singleOrDefault = _session.QueryOver<OrderLine>()
                .Where(line => line.ProductVariant.Id.IsIn(values))
                .SelectList(
                    builder =>
                        builder.SelectSum(line => line.Quantity)
                            .WithAlias(() => numberBoughtCount.Count))
                .TransformUsing(Transformers.AliasToBean<NumberBoughtCount>())
                .SingleOrDefault<NumberBoughtCount>();
            return singleOrDefault != null ? singleOrDefault.Count : 0;
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

    internal class NumberBoughtCount
    {
        public int Count { get; set; }
    }
}