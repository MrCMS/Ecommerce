using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;
using NHibernate.Criterion;
using System.Collections.Generic;
using System.Linq;
namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly ISession _session;

        public ProductVariantService(ISession session)
        {
            _session = session;
        }
        public ProductVariant GetProductVariantBySKU(string sku)
        {
            return _session.QueryOver<ProductVariant>()
                            .Where(
                                variant =>
                                variant.SKU.IsInsensitiveLike(sku, MatchMode.Exact)).SingleOrDefault();
        }
        public ProductVariant Get(int id)
        {
            return _session.QueryOver<ProductVariant>()
                            .Where(
                                variant =>
                                variant.Id == id).SingleOrDefault();
        }

        public List<SelectListItem> GetOptions()
        {
            return _session.QueryOver<ProductVariant>()
                           .Cacheable()
                           .List()
                           .BuildSelectItemList(item => item.Name,
                                                item => item.Id.ToString(),
                                                emptyItemText: null);
        }

        public PriceBreak AddPriceBreak(AddPriceBreakModel model)
        {
            var productVariant = _session.Get<ProductVariant>(model.Id);
            var priceBreak = new PriceBreak
            {
                Item = productVariant,
                Quantity = model.Quantity,
                Price = model.Price
            };

            _session.Transact(session =>
            {
                session.SaveOrUpdate(priceBreak);
                session.SaveOrUpdate(productVariant);
            });

            return priceBreak;
        }

        public void DeletePriceBreak(PriceBreak priceBreak)
        {
            _session.Transact(session => session.Delete(priceBreak));
        }

        public void Add(ProductVariant productVariant)
        {
            if (productVariant.Product != null)
            {
                productVariant.Product.Variants.Add(productVariant);

                foreach (ProductAttributeValue t in productVariant.AttributeValues)
                {
                    t.ProductVariant = productVariant;
                }
                _session.Transact(session => session.Save(productVariant));
            }
        }

        public void Update(ProductVariant productVariant)
        {
            _session.Transact(session => session.Update(productVariant));
        }

        public void Delete(ProductVariant productVariant)
        {
            _session.Transact(session => session.Delete(productVariant));
        }

        public bool AnyExistingProductVariantWithSKU(string sku, ProductVariant productVariant)
        {
            return _session.QueryOver<ProductVariant>()
                           .Where(
                               variant =>
                               variant.SKU.IsInsensitiveLike(sku, MatchMode.Exact) && variant.Id != productVariant.Id)
                           .RowCount() > 0;
        }

        public bool IsPriceBreakQuantityValid(int quantity, ProductVariant productVariant)
        {
            return quantity > 1 && productVariant.PriceBreaks.All(@break => @break.Quantity != quantity);
        }

        public bool IsPriceBreakPriceValid(decimal price, ProductVariant productVariant, int quantity)
        {
            var priceBreaks = productVariant.PriceBreaks;
            return price < productVariant.BasePrice && price > 0
                   && priceBreaks.Where(@break => @break.Quantity < quantity).All(@break => @break.Price > price)
                   && priceBreaks.Where(@break => @break.Quantity > quantity).All(@break => @break.Price < price);
        }
    }
}