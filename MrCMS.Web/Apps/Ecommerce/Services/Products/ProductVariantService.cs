using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;
using System.Collections.Generic;
using System.Linq;
using NHibernate.SqlCommand;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly ISession _session;

        public ProductVariantService(ISession session)
        {
            _session = session;
        }
        public IList<ProductVariant> GetAll()
        {
            return _session.QueryOver<ProductVariant>().Cacheable().List();
        }

        public IList<ProductVariant> GetAllVariantsWithLowStock(int treshold)
        {
            return _session.QueryOver<ProductVariant>().Where(item => item.StockRemaining <= treshold
                && item.TrackingPolicy == TrackingPolicy.Track).OrderBy(x => x.Product.Id).Asc.Cacheable().List();
        }
        public IList<ProductVariant> GetAllVariantsForStockReport()
        {
            return _session.QueryOver<ProductVariant>().OrderBy(x => x.Product.Id).Asc.Cacheable().List();
        }
        public IPagedList<ProductVariant> GetAllVariants(string queryTerm, int categoryId = 0, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(queryTerm) && categoryId == 0)
                return _session.Paged(QueryOver.Of<ProductVariant>().Cacheable(), page, MrCMSApplication.Get<SiteSettings>().DefaultPageSize);

            Category categoryAlias = null;
            Product productAlias = null;
            ProductVariant productVariantAlias = null;
            return _session.QueryOver(() => productVariantAlias)
                                .JoinAlias(() => productVariantAlias.Product, () => productAlias, JoinType.InnerJoin)
                                .JoinAlias(() => productAlias.Categories, () => categoryAlias, JoinType.LeftOuterJoin)
                                .Where(
                                    () => (productVariantAlias.Name.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)
                                        || productVariantAlias.SKU.IsInsensitiveLike(queryTerm, MatchMode.Anywhere))
                                    && (categoryId == 0 || categoryAlias.Id == categoryId))
                                .Paged(page, MrCMSApplication.Get<SiteSettings>().DefaultPageSize);
        }
        public IList<ProductVariant> GetAllVariantsForGoogleBase()
        {
            return _session.QueryOver<ProductVariant>().JoinQueryOver(x => x.Product, JoinType.LeftOuterJoin).Where(variant => variant.PublishOn != null).Cacheable().List();
        }
        public ProductVariant GetProductVariantBySKU(string sku)
        {
            var trim = sku.Trim();
            return _session.QueryOver<ProductVariant>()
                           .Where(
                               variant =>
                               variant.SKU == trim).Take(1).Cacheable().SingleOrDefault();
        }

        public ProductVariant Get(int id)
        {
            return _session.QueryOver<ProductVariant>()
                            .Where(
                                variant =>
                                variant.Id == id).Cacheable().SingleOrDefault();
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
                ProductVariant = productVariant,
                Quantity = model.Quantity,
                Price = model.Price
            };
            productVariant.PriceBreaks.Add(priceBreak);
            _session.Transact(session => session.SaveOrUpdate(priceBreak));
            return priceBreak;
        }

        public PriceBreak AddPriceBreak(PriceBreak item)
        {
            _session.Transact(session => session.Save(item));

            return item;
        }

        public void DeletePriceBreak(PriceBreak priceBreak)
        {
            if (priceBreak.ProductVariant != null)
            {
                priceBreak.ProductVariant.PriceBreaks.Remove(priceBreak);
            }
            _session.Transact(session => session.Delete(priceBreak));
        }

        public bool AnyExistingProductVariantWithSKU(string sku, int id)
        {
            var trim = sku.Trim();
            return _session.QueryOver<ProductVariant>()
                           .Where(
                               variant =>
                               variant.SKU == trim && variant.Id != id)
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