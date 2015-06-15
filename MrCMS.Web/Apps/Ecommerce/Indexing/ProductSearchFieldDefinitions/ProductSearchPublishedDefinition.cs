using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchPublishedDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        public ProductSearchPublishedDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "published", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            yield return obj.Published.ToString();
        }
        private static BooleanClause _clause;

        public static BooleanClause PublishedOnly
        {
            get
            {
                return _clause = _clause ?? new BooleanClause(
                    new TermQuery(new Term(FieldDefinition.GetFieldName<ProductSearchPublishedDefinition>(), Boolean.TrueString)),
                    Occur.MUST);

            }
        }
    }
}