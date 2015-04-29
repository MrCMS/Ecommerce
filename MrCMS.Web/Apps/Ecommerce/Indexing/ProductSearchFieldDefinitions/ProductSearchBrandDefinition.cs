using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Indexing;
using MrCMS.Indexing.Management;
using MrCMS.Tasks;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using System.Linq;
using Brand = MrCMS.Web.Apps.Ecommerce.Pages.Brand;

namespace MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions
{
    public class ProductSearchBrandDefinition : StringFieldDefinition<ProductSearchIndex, Product>
    {
        private readonly ISession _session;

        public ProductSearchBrandDefinition(ISession session, ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "brands", index: Field.Index.NOT_ANALYZED)
        {
            _session = session;
        }

        protected override IEnumerable<string> GetValues(Product obj)
        {
            if (obj.BrandPage != null)
                yield return obj.BrandPage.Id.ToString();

            yield return null;
        }

        public override Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>> GetRelatedEntities()
        {
            return new Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>>
                       {
                           {
                               typeof (Brand),
                               entity =>
                                   {
                                       if (entity is Brand)
                                       {
                                           var page = (entity as Brand);
                                           var products =
                                               _session.QueryOver<Product>()
                                                       .Where(product => product.BrandPage.Id == page.Id)
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