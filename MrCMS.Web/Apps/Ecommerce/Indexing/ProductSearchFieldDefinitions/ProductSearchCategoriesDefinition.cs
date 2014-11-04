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
            var categories = new HashSet<Category>(_session.QueryOver<Category>().Fetch(category => category.Products).Eager.List());

            return objs.ToDictionary(product => product, product => GetCategories(product, categories));
        }

        private IEnumerable<string> GetCategories(Product product, HashSet<Category> categories)
        {
            foreach (var category in categories.Where(category => category.Products.Contains(product)))
            {
                yield return category.Id.ToString();
                var parent = categories.FirstOrDefault(c => c.Id == category.ParentId);
                while (parent != null)
                {
                    yield return parent.Id.ToString();
                    parent = categories.FirstOrDefault(c => c.Id == parent.ParentId);
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