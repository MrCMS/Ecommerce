using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using NHibernate.Transform;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductBarcodeDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        private readonly ISession _session;

        public ProductBarcodeDefinition(ILuceneSettingsService luceneSettingsService, ISession session)
            : base(luceneSettingsService, "barcode", index: Field.Index.ANALYZED)
        {
            _session = session;
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            return GetBarcodes(obj.Variants);
        }

        private static IEnumerable<string> GetBarcodes(IEnumerable<ProductVariant> productVariants)
        {
            return productVariants.Select(x => x.Barcode).Where(x => !string.IsNullOrWhiteSpace(x));
        }

        protected override Dictionary<Product, IEnumerable<string>> GetValues(List<Product> objs)
        {
            BarcodeList list = null;
            IList<BarcodeList> partNumberLists = _session.QueryOver<ProductVariant>()
                .Where(x => x.Barcode != null && x.Barcode != "")
                .SelectList(builder => builder
                    .Select(variant => variant.Barcode).WithAlias(() => list.Barcode)
                    .Select(variant => variant.Product.Id).WithAlias(() => list.ProductId)
                )
                .TransformUsing(Transformers.AliasToBean<BarcodeList>())
                .Cacheable()
                .List<BarcodeList>();
            Dictionary<int, IEnumerable<string>> dictionary = partNumberLists.GroupBy(skuList => skuList.ProductId)
                .ToDictionary(lists => lists.Key, lists => lists.Select(partNumberList => partNumberList.Barcode));

            return objs.ToDictionary(product => product,
                product => dictionary.ContainsKey(product.Id) ? dictionary[product.Id] : Enumerable.Empty<string>());
        }

        public class BarcodeList
        {
            public string Barcode { get; set; }
            public int ProductId { get; set; }
        }
    }
}