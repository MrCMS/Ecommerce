using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Indexing;
using MrCMS.Indexing.Management;
using MrCMS.Tasks;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using NHibernate.Transform;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchCategoriesDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        private readonly ISession _session;

        public ProductSearchCategoriesDefinition(ISession session, ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "categories", index: Field.Index.NOT_ANALYZED)
        {
            _session = session;
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            return GetCategories(obj.Categories);
        }

        protected override Dictionary<Product, IEnumerable<string>> GetValues(List<Product> objs)
        {
            var allCategories = _session.QueryOver<Category>().Cacheable().List().ToDictionary(x => x.Id);

            Category categoryAlias = null;
            CategoryMapping mapping = null;
            var mappings = _session.QueryOver<Product>()
                .JoinAlias(x => x.Categories, () => categoryAlias)
                .SelectList(builder =>
                {
                    builder.Select(x => x.Id).WithAlias(() => mapping.ProductId);
                    builder.Select(() => categoryAlias.Id).WithAlias(() => mapping.CategoryId);
                    return builder;
                }).TransformUsing(Transformers.AliasToBean<CategoryMapping>())
                .Cacheable()
                .List<CategoryMapping>()
                .GroupBy(x => x.ProductId, x => x.CategoryId)
                .ToDictionary(x => x.Key);

            return objs.ToDictionary(product => product, product => GetCategories(mappings.ContainsKey(product.Id)
                ? mappings[product.Id] : Enumerable.Empty<int>(), allCategories));
        }

        private class CategoryMapping
        {
            public int ProductId { get; set; }
            public int CategoryId { get; set; }
        }

        private IEnumerable<string> GetCategories(IEnumerable<int> categoryIds, Dictionary<int, Category> allCategories)
        {
            foreach (var category in categoryIds.Select(id => allCategories[id]))
            {
                yield return category.Id.ToString();
                var parent = allCategories.ContainsKey(category.ParentId) ? allCategories[category.ParentId] : null;
                while (parent != null)
                {
                    yield return parent.Id.ToString();
                    parent = allCategories.ContainsKey(parent.ParentId) ? allCategories[parent.ParentId] : null;
                }
            }
        }

        private static IEnumerable<string> GetCategories(IEnumerable<Category> categories)
        {
            var list = new List<Category>();
            list.AddRange(categories.SelectMany(GetCategoryHierarchy));
            var enumerable = list.Distinct().Select(category => category.Id.ToString()).ToList();
            return enumerable;
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

        public override Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>> GetRelatedEntities()
        {
            return new Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>>
                       {
                           {
                               typeof (Category),
                               entity =>
                                   {
                                       if (entity is Category)
                                       {
                                           var category = (entity as Category);
                                           Category categoryAlias = null;
                                           IList<Product> products =
                                               _session.QueryOver<Product>()
                                                       .JoinAlias(product => product.Categories, () => categoryAlias)
                                                       .Where(product => categoryAlias.Id == category.Id)
                                                       .Cacheable()
                                                       .List();
                                           return products.Select(product =>
                                                                  new LuceneAction
                                                                      {
                                                                          Entity = product.Unproxy(),
                                                                          Operation = LuceneOperation.Update,
                                                                          IndexDefinition =
                                                                              IndexingHelper.Get<ProductSearchIndex>()
                                                                      }).ToList();
                                       }
                                       return new List<LuceneAction>();
                                   }
                           }
                       };
        }
    }
}