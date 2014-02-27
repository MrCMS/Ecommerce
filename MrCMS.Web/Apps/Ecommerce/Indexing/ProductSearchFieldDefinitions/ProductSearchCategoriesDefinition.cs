using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchCategoriesDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        public ProductSearchCategoriesDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "categories", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            return GetCategories(obj.Categories);
        }

        private static IEnumerable<string> GetCategories(IEnumerable<Category> categories)
        {
            var list = new List<Category>();
            list.AddRange(categories.SelectMany(GetCategoryHierarchy));
            return list.Distinct().Select(category => category.Id.ToString());
        }

        private static IEnumerable<Category> GetCategoryHierarchy(Category category)
        {
            yield return category;
            var parent = category.Parent.Unproxy() as Category;
            while (parent != null)
            {
                yield return parent;
                parent = parent.Parent.Unproxy() as Category;
            }
        }
    }
}