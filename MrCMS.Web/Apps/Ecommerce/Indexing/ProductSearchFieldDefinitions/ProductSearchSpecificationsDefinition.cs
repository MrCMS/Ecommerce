using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Entities;
using MrCMS.Indexing;
using MrCMS.Indexing.Management;
using MrCMS.Tasks;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchSpecificationsDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        public ProductSearchSpecificationsDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "specifications", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            return
                obj.SpecificationValues.Select(value => value.ProductSpecificationAttributeOption.Id.ToString())
                   .Distinct();
        }

        public override Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>> GetRelatedEntities()
        {
            return new Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>>
                       {
                           {
                               typeof (ProductSpecificationAttributeOption),
                               GetActions
                           }
                       };
        }

        private static IEnumerable<LuceneAction> GetActions(SystemEntity entity)
        {
            var line = entity as ProductSpecificationAttributeOption;
            if (line == null)
                yield break;

            var productSpecificationValues = line.Values;
            foreach (var product in productSpecificationValues.Select(value => value.Product).Where(p => p != null))
            {
                yield return new LuceneAction
                                 {
                                     Entity = product.Unproxy(),
                                     Operation = LuceneOperation.Update,
                                     IndexDefinition =
                                         IndexingHelper.Get<ProductSearchIndex>()
                                 };
            }
        }
    }
}