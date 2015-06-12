using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Indexing.Querying;
using MrCMS.Services.Notifications;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Indexing;
using MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using NHibernate.Transform;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class CategoryProductsAdminService : ICategoryProductsAdminService
    {
        private readonly ISession _session;
        private readonly ISearcher<Product, ProductSearchIndex> _productSearcher;

        public CategoryProductsAdminService(ISession session, ISearcher<Product, ProductSearchIndex> productSearcher)
        {
            _session = session;
            _productSearcher = productSearcher;
        }

        public List<ProductSortData> GetProductSortData(Category category)
        {
            if (category == null)
                return new List<ProductSortData>();
            var products = GetProducts(category);
            ProductSortData productSortDataAlias = null;
            Product productAlias = null;
            var orders =
                _session.QueryOver<CategoryProductDisplayOrder>().Where(order => order.Category.Id == category.Id)
                .JoinAlias(order => order.Product, () => productAlias)
                    .OrderBy(order => order.DisplayOrder).Asc.SelectList(builder =>
                        builder
                            .Select(order => order.DisplayOrder)
                            .WithAlias(() => productSortDataAlias.DisplayOrder)
                            .Select(order => productAlias.Id)
                            .WithAlias(() => productSortDataAlias.Id)
                            .Select(order => productAlias.Name)
                            .WithAlias(() => productSortDataAlias.Name))
                    .TransformUsing(Transformers.AliasToBean<ProductSortData>())
                    .List<ProductSortData>().ToHashSet();

            return products.Select(product => orders.FirstOrDefault(order => order.Id == product.Id)
                                              ??
                                              new ProductSortData
                                              {
                                                  Id = product.Id,
                                                  Name = product.Name,
                                                  DisplayOrder = products.Count
                                              }).OrderBy(data => data.DisplayOrder).ToList();
        }

        public bool IsSorted(Category category)
        {
            return
                _session.QueryOver<CategoryProductDisplayOrder>()
                    .Where(order => order.Category.Id == category.Id)
                    .Cacheable()
                    .Any();
        }

        private List<Product> GetProducts(Category category)
        {
            var booleanQuery = new BooleanQuery
            {
                {
                    new TermQuery(new Term(FieldDefinition.GetFieldName<ProductSearchCategoriesDefinition>(),
                        category.Id.ToString())),
                    Occur.MUST
                }
            };

            return _productSearcher.Search(booleanQuery, 1, int.MaxValue).ToList();
        }

        public void Update(Category category, List<ProductSortData> productSortData)
        {
            var categoryProductDisplayOrders =
                _session.QueryOver<CategoryProductDisplayOrder>()
                    .Where(order => order.Category.Id == category.Id)
                    .Cacheable()
                    .List();

            using (new NotificationDisabler())
            {
                _session.Transact(session =>
                {
                    foreach (var sortData in productSortData)
                    {
                        var displayOrder =
                            categoryProductDisplayOrders.FirstOrDefault(order => order.Product.Id == sortData.Id);
                        if (displayOrder != null)
                        {
                            displayOrder.DisplayOrder = sortData.DisplayOrder;
                            session.Update(displayOrder);
                            continue;
                        }
                        displayOrder = new CategoryProductDisplayOrder
                        {
                            Category = category,
                            Product = session.Get<Product>(sortData.Id),
                            DisplayOrder = sortData.DisplayOrder
                        };
                        session.Save(displayOrder);
                    }
                });
            }
        }

        public void ClearSorting(Category category)
        {
            using (new NotificationDisabler())
            {
                _session.Transact(session =>
                {
                    var orders =
                        _session.QueryOver<CategoryProductDisplayOrder>()
                            .Where(order => order.Category.Id == category.Id)
                            .Cacheable()
                            .List();
                    foreach (var order in orders)
                    {
                        session.Delete(order);
                    }
                });
            }
        }
    }
}