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
    public class ProductPartNumberDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        private readonly ISession _session;

        public ProductPartNumberDefinition(ILuceneSettingsService luceneSettingsService, ISession session)
            : base(luceneSettingsService, "part-number", index: Field.Index.ANALYZED)
        {
            _session = session;
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            return GetPartNumbers(obj.Variants);
        }

        private static IEnumerable<string> GetPartNumbers(IEnumerable<ProductVariant> productVariants)
        {
            return productVariants.Select(x => x.ManufacturerPartNumber).Where(x => !string.IsNullOrWhiteSpace(x));
        }

        protected override Dictionary<Product, IEnumerable<string>> GetValues(List<Product> objs)
        {
            PartNumberList list = null;
            IList<PartNumberList> partNumberLists = _session.QueryOver<ProductVariant>()
                .Where(x => x.ManufacturerPartNumber != null && x.ManufacturerPartNumber != "")
                .SelectList(builder => builder
                    .Select(variant => variant.ManufacturerPartNumber).WithAlias(() => list.PartNumber)
                    .Select(variant => variant.Product.Id).WithAlias(() => list.ProductId)
                )
                .TransformUsing(Transformers.AliasToBean<PartNumberList>())
                .List<PartNumberList>();
            Dictionary<int, IEnumerable<string>> dictionary = partNumberLists.GroupBy(skuList => skuList.ProductId)
                .ToDictionary(lists => lists.Key, lists => lists.Select(partNumberList => partNumberList.PartNumber));

            return objs.ToDictionary(product => product,
                product => dictionary.ContainsKey(product.Id) ? dictionary[product.Id] : Enumerable.Empty<string>());
        }

        public class PartNumberList
        {
            public string PartNumber { get; set; }
            public int ProductId { get; set; }
        }
    }
}