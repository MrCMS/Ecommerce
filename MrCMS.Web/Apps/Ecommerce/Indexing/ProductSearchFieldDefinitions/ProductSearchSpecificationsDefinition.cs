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
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchSpecificationsDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        private readonly ISession _session;

        public ProductSearchSpecificationsDefinition(ILuceneSettingsService luceneSettingsService, ISession session)
            : base(luceneSettingsService, "specifications", index: Field.Index.NOT_ANALYZED)
        {
            _session = session;
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            return
                obj.SpecificationValues.Select(value => value.ProductSpecificationAttributeOption.Id.ToString())
                   .Distinct();
        }

        protected override Dictionary<Product, IEnumerable<string>> GetValues(List<Product> objs)
        {
            var values =
                new HashSet<ProductSpecificationValue>(_session.QueryOver<ProductSpecificationValue>()
                    .Fetch(value => value.ProductSpecificationAttributeOption)
                    .Eager.List());

            return objs.ToDictionary(product => product, product => GetValues(product, values));
        }

        private IEnumerable<string> GetValues(Product product, HashSet<ProductSpecificationValue> values)
        {
            return
                values.Where(value => value.Product != null && value.Product.Id == product.Id)
                    .Select(value => value.ProductSpecificationAttributeOption.Id.ToString())
                    .ToList();
        }

        public override Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>> GetRelatedEntities()
        {
            return new Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>>
                       {
                           {
                               typeof (ProductSpecificationAttributeOption),
                               GetActions
                           },
                           {
                               typeof (ProductSpecificationValue),
                               GetValueActions
                           }
                       };
        }

        private IEnumerable<LuceneAction> GetValueActions(SystemEntity entity)
        {
            var line = entity as ProductSpecificationValue;
            if (line == null)
                yield break;

            yield return new LuceneAction
            {
                Entity = line.Product.Unproxy(),
                Operation = LuceneOperation.Update,
                IndexDefinition = IndexingHelper.Get<ProductSearchIndex>()
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