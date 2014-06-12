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

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchOptionsDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        public ProductSearchOptionsDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "options", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            return
                obj.Variants.SelectMany(variant => variant.OptionValues)
                    .Select(i => string.Format("{0}[{1}]", i.ProductOption.Id, i.Value));
        }

        public override Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>> GetRelatedEntities()
        {
            return new Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>>
                       {
                           {
                               typeof (ProductOptionValue),
                               GetActions
                           }
                       };
        }

        private static IEnumerable<LuceneAction> GetActions(SystemEntity entity)
        {
            var line = entity as ProductOptionValue;
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