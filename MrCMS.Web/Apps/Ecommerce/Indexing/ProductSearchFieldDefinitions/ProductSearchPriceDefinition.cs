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
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Settings;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchPriceDefinition : DecimalFieldDefinition<ProductSearchIndex, Product>
    {
        private readonly TaxSettings _taxSettings;
        private readonly ISession _session;

        public ProductSearchPriceDefinition(ILuceneSettingsService luceneSettingsService, TaxSettings taxSettings, ISession session)
            : base(luceneSettingsService, "price", index: Field.Index.NOT_ANALYZED)
        {
            _taxSettings = taxSettings;
            _session = session;
        }

        protected override IEnumerable<decimal> GetValues(Product obj)
        {
            return GetPrices(obj);
        }

        public IEnumerable<decimal> GetPrices(Product entity)
        {
            return entity.Variants.Select(pv => pv.Price);
        }

        public override Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>> GetRelatedEntities()
        {
            return new Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>>
                   {
                       {typeof(ProductVariant),GetVariantActions},
                       {typeof(TaxRate),GetTaxRateActions}
                   };
        }

        private IEnumerable<LuceneAction> GetTaxRateActions(SystemEntity entity)
        {
            if (!_taxSettings.TaxesEnabled || _taxSettings.LoadedPricesIncludeTax)
                yield break;

            var rate = entity as TaxRate;
            if (rate == null)
                yield break;

            var variants = _session.QueryOver<ProductVariant>().Where(variant => variant.TaxRate.Id == rate.Id && variant.Product != null).List().ToList();

            if (rate.IsDefault)
            {
                variants.AddRange(
                    _session.QueryOver<ProductVariant>()
                        .Where(variant => variant.TaxRate == null && variant.Product != null)
                        .List());
            }
            foreach (var variant in variants)
            {
                yield return GetAction(variant.Product);
            }
        }

        private IEnumerable<LuceneAction> GetVariantActions(SystemEntity entity)
        {
            var variant = entity as ProductVariant;
            if (variant != null && variant.Product != null)
                yield return GetAction(variant.Product);
        }

        private static LuceneAction GetAction(Product product)
        {
            return new LuceneAction
                   {
                       Entity = product.Unproxy(),
                       Operation = LuceneOperation.Update,
                       IndexDefinition = IndexingHelper.Get<ProductSearchIndex>()
                   };
        }
    }
}