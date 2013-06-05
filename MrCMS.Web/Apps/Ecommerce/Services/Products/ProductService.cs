using System;
using System.Linq;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using MrCMS.Helpers;
using NHibernate.Criterion;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly ISession _session;
        private readonly IDocumentService _documentService;

        public ProductService(ISession session, IDocumentService documentService)
        {
            _session = session;
            _documentService = documentService;
        }

        public ProductPagedList Search(string queryTerm = null, int page = 1)
        {
            IPagedList<Product> pagedList;
            if (!string.IsNullOrWhiteSpace(queryTerm))
            {
                pagedList =
                    _session.Paged(
                        QueryOver.Of<Product>()
                                 .Where(product => product.Name.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)), page,
                        10);
            }
            else
            {
                pagedList = _session.Paged(QueryOver.Of<Product>(), page, 10);
            }

            var productContainer = _documentService.GetUniquePage<ProductSearch>();
            var productContainerId = productContainer == null ? (int?)null : productContainer.Id;
            return new ProductPagedList(pagedList, productContainerId);
        }

        public void MakeMultiVariant(MakeMultivariantModel model)
        {
            var product = _documentService.GetDocument<Product>(model.ProductId);
            if (!string.IsNullOrWhiteSpace(model.Option1))
            {
                AddAttributeOption(model.Option1, product);
            }
            if (!string.IsNullOrWhiteSpace(model.Option2))
            {
                AddAttributeOption(model.Option2, product);
            }
            if (!string.IsNullOrWhiteSpace(model.Option3))
            {
                AddAttributeOption(model.Option3, product);
            }
            var productVariant = new ProductVariant { Product = product };
            product.Variants.Add(productVariant);
            var productAttributeValues = GetAttributeValues(model, productVariant);
            productAttributeValues.ForEach(value => productVariant.AttributeValues.Add(value));

            _session.Transact(session =>
                                  {
                                      session.SaveOrUpdate(product);
                                      product.Variants.ForEach(variant =>
                                                                   {
                                                                       session.SaveOrUpdate(variant);
                                                                       variant.AttributeValues.ForEach(session.SaveOrUpdate);
                                                                   });
                                      product.AttributeOptions.ForEach(session.SaveOrUpdate);
                                  });
        }

        private IList<ProductAttributeValue> GetAttributeValues(MakeMultivariantModel model, ProductVariant productVariant)
        {
            var values = new List<ProductAttributeValue>();
            if (!string.IsNullOrWhiteSpace(model.Option1))
                values.Add(new ProductAttributeValue
                               {
                                   ProductAttributeOption =
                                       _session.QueryOver<ProductAttributeOption>()
                                               .Where(
                                                   option =>
                                                   option.Name.IsInsensitiveLike(model.Option1, MatchMode.Exact))
                                               .SingleOrDefault(),
                                   Value = model.Option1Value,
                                   ProductVariant = productVariant
                               });
            if (!string.IsNullOrWhiteSpace(model.Option2))
                values.Add(new ProductAttributeValue
                               {
                                   ProductAttributeOption =
                                       _session.QueryOver<ProductAttributeOption>()
                                               .Where(
                                                   option =>
                                                   option.Name.IsInsensitiveLike(model.Option2, MatchMode.Exact))
                                               .SingleOrDefault(),
                                   Value = model.Option2Value,
                                   ProductVariant = productVariant
                               });
            if (!string.IsNullOrWhiteSpace(model.Option3))
                values.Add(new ProductAttributeValue
                               {
                                   ProductAttributeOption =
                                       _session.QueryOver<ProductAttributeOption>()
                                               .Where(
                                                   option =>
                                                   option.Name.IsInsensitiveLike(model.Option3, MatchMode.Exact))
                                               .SingleOrDefault(),
                                   Value = model.Option3Value,
                                   ProductVariant = productVariant
                               });
            return values;
        }

        private void AddAttributeOption(string optionName, Product product)
        {
            var productAttributeOption =
                _session.QueryOver<ProductAttributeOption>()
                        .Where(option => option.Name.IsInsensitiveLike(optionName, MatchMode.Exact))
                        .Take(1)
                        .SingleOrDefault();
            if (productAttributeOption == null)
            {
                _session.Transact(session => session.Save(new ProductAttributeOption() { Name=optionName }));
                productAttributeOption =
                _session.QueryOver<ProductAttributeOption>()
                        .Where(option => option.Name.IsInsensitiveLike(optionName, MatchMode.Exact))
                        .Take(1)
                        .SingleOrDefault();
            }
            product.AttributeOptions.Add(productAttributeOption);
        }

        public void AddCategory(Product product, int categoryId)
        {
            var category = _documentService.GetDocument<Category>(categoryId);
            product.Categories.Add(category);
            category.Products.Add(product);
            _session.Transact(session => session.SaveOrUpdate(product));
        }

        public void RemoveCategory(Product product, int categoryId)
        {
            var category = _documentService.GetDocument<Category>(categoryId);
            product.Categories.Remove(category);
            category.Products.Remove(product);
            _session.Transact(session => session.SaveOrUpdate(product));
        }

        public List<SelectListItem> GetOptions()
        {
            return _session.QueryOver<Product>().Cacheable().List().BuildSelectItemList(item => item.Name, item => item.Id.ToString(), emptyItemText: null);
        }

        public Product Get(int id)
        {
            return _session.QueryOver<Product>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }

        public Product GetByName(string name)
        {
            return _session.QueryOver<Product>()
                           .Where(
                               product =>
                               product.Name.IsInsensitiveLike(name, MatchMode.Exact))
                           .SingleOrDefault();
        }
        public IList<Product> GetAll()
        {
            return _session.QueryOver<Product>().Cacheable().List();
        }

        public PriceBreak AddPriceBreak(AddPriceBreakModel model)
        {
            switch (model.Type)
            {
                case "Product":
                    return AddPriceBreakForProduct(model);
                case "ProductVariant":
                    return AddPriceBreakForProductVariant(model);
                default:
                    throw new IndexOutOfRangeException("model");
            }
        }

        private PriceBreak AddPriceBreakForProductVariant(AddPriceBreakModel model)
        {
            var productVariant = _session.Get<ProductVariant>(model.Id);
            var priceBreak = new PriceBreak
            {
                Item = productVariant,
                Quantity = model.Quantity,
                Price = model.Price
            };
            productVariant.PriceBreaks.Add(priceBreak);

            _session.Transact(session =>
            {
                session.SaveOrUpdate(productVariant);
                session.SaveOrUpdate(priceBreak);
            });
            return priceBreak;
        }

        private PriceBreak AddPriceBreakForProduct(AddPriceBreakModel model)
        {
            var product = _session.Get<Product>(model.Id);
            var priceBreak = new PriceBreak
                                 {
                                     Item = product,
                                     Quantity = model.Quantity,
                                     Price = model.Price
                                 };
            product.PriceBreaks.Add(priceBreak);

            _session.Transact(session =>
                                  {
                                      session.SaveOrUpdate(product);
                                      session.SaveOrUpdate(priceBreak);
                                  });
            return priceBreak;
        }

        public void DeletePriceBreak(PriceBreak priceBreak)
        {
            _session.Transact(session => session.Delete(priceBreak));
        }

        public bool IsPriceBreakQuantityValid(int quantity, int id, string type)
        {
            var priceBreaks = GetExistingPriceBreaks(id, type);

            return quantity > 1 && priceBreaks.All(@break => @break.Quantity != quantity);
        }

        public bool IsPriceBreakPriceValid(decimal price, int id, string type, int quantity)
        {
            var basePrice = GetExistingBasePrice(id, type);
            var priceBreaks = GetExistingPriceBreaks(id, type);
            return price < basePrice && price > 0
                   && priceBreaks.Where(@break => @break.Quantity < quantity).All(@break => @break.Price > price)
                   && priceBreaks.Where(@break => @break.Quantity > quantity).All(@break => @break.Price < price);
        }

        private decimal GetExistingBasePrice(int id, string type)
        {
            switch (type)
            {
                case "Product":
                    return _session.Get<Product>(id).BasePrice;
                case "ProductVariant":
                    return _session.Get<ProductVariant>(id).BasePrice;
                default:
                    throw new IndexOutOfRangeException("type");
            }
        }

        private IList<PriceBreak> GetExistingPriceBreaks(int id, string type)
        {
            switch (type)
            {
                case "Product":
                    return _session.Get<Product>(id).PriceBreaks;
                case "ProductVariant":
                    return _session.Get<ProductVariant>(id).PriceBreaks;
                default:
                    throw new IndexOutOfRangeException("type");
            }
        }

        public bool AnyExistingProductWithSKU(string sku, int id)
        {
            return _session.QueryOver<Product>()
                           .Where(
                               product =>
                               product.SKU.IsInsensitiveLike(sku, MatchMode.Exact) && product.Id!=id)
                           .RowCount() > 0;
        }
    }
}