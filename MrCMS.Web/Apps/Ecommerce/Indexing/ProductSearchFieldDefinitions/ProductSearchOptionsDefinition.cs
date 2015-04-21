using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Indexing;
using MrCMS.Indexing.Management;
using MrCMS.Tasks;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using NHibernate.Transform;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchOptionsDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        private readonly ISession _session;

        public ProductSearchOptionsDefinition(ILuceneSettingsService luceneSettingsService, ISession session)
            : base(luceneSettingsService, "options", index: Field.Index.NOT_ANALYZED)
        {
            _session = session;
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            return
                obj.Variants.SelectMany(variant => variant.OptionValues)
                    .Select(i => OptionValueData.GetValue(i.ProductOption.Id, i.Value));
        }

        protected override Dictionary<Product, IEnumerable<string>> GetValues(List<Product> objs)
        {
            ProductVariant productVariantAlias = null;
            OptionValueData data = null;
            IList<OptionValueData> values = _session.QueryOver<ProductOptionValue>()
                .JoinAlias(value => value.ProductVariant, () => productVariantAlias)
                .SelectList(builder => builder
                    .Select(value => value.Value).WithAlias(() => data.Value)
                    .Select(value => value.ProductOption.Id).WithAlias(() => data.OptionId)
                    .Select(() => productVariantAlias.Product.Id).WithAlias(() => data.ProductId)
                ).TransformUsing(Transformers.AliasToBean<OptionValueData>())
                .List<OptionValueData>();

            Dictionary<int, IEnumerable<string>> dictionary =
                values.GroupBy(valueData => valueData.ProductId)
                    .ToDictionary(datas => datas.Key, datas => datas.Select(valueData => valueData.TermValue));

            return objs.ToDictionary(product => product,
                product => dictionary.ContainsKey(product.Id) ? dictionary[product.Id] : Enumerable.Empty<string>());
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

        private class OptionValueData
        {
            public string Value { get; set; }
            public int OptionId { get; set; }
            public int ProductId { get; set; }

            public string TermValue
            {
                get { return GetValue(OptionId, Value); }
            }

            public static string GetValue(int optionId, string value)
            {
                return string.Format("{0}[{1}]", optionId, value);
            }
        }
    }
}