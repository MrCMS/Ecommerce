using System;
using System.Collections.Generic;
using MarketplaceWebServiceFeedsClasses;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Amazon.Services.Listings
{
    public class AmazonListingService : IAmazonListingService
    {
        private readonly ISession _session;
        private readonly EcommerceSettings _ecommerceSettings;

        public AmazonListingService(ISession session, EcommerceSettings ecommerceSettings)
        {
            _session = session;
            _ecommerceSettings = ecommerceSettings;
        }

        public AmazonListing Get(int id)
        {
            return _session.QueryOver<AmazonListing>()
                            .Where(item => item.Id == id).SingleOrDefault();
        }

        public AmazonListing GetByAmazonListingId(string id)
        {
            return _session.QueryOver<AmazonListing>()
                            .Where(item => item.AmazonListingId.IsInsensitiveLike(id,MatchMode.Exact)).SingleOrDefault();
        }

        public AmazonListing GetByProductVariantId(int id)
        {
            return _session.QueryOver<AmazonListing>()
                            .Where(item => item.ProductVariant.Id==id).SingleOrDefault();
        }

        public IEnumerable<AmazonListing> GetAll()
        {
            return _session.QueryOver<AmazonListing>().Cacheable().List();
        }

        public IPagedList<AmazonListing> Search(string queryTerm = null, int page = 1, int pageSize = 10)
        {
            if (!string.IsNullOrWhiteSpace(queryTerm))
            {
                return _session.QueryOver<AmazonListing>()
                                    .Where(x => 
                                        x.AmazonListingId.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)
                                        || x.SellerSKU.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)
                                        || x.ASIN.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)
                                        || x.Title.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)
                                        ).Paged(page, pageSize);
            }

            return _session.Paged(QueryOver.Of<AmazonListing>(), page, pageSize);
        }

        public void Save(AmazonListing item)
        {
            _session.Transact(session => session.SaveOrUpdate(item));
        }

        public void Delete(AmazonListing item)
        {
            _session.Transact(session => session.Delete(item));
        }

        public AmazonListing InitAmazonListingFromProductVariant(ProductVariant productVariant)
        {
            return new AmazonListing()
            {
                ProductVariant = productVariant,
                Brand =
                    productVariant.Product.Brand != null ? productVariant.Product.Brand.Name : String.Empty,
                Condition = ConditionType.New,
                Currency = _ecommerceSettings.Currency.Code,
                Manafacturer =
                    productVariant.Product.Brand != null ? productVariant.Product.Brand.Name : String.Empty,
                MfrPartNumber = productVariant.ManufacturerPartNumber,
                Quantity =
                    productVariant.StockRemaining.HasValue
                        ? Decimal.ToInt32(productVariant.StockRemaining.Value)
                        : 1,
                Price = productVariant.Price,
                SellerSKU = productVariant.SKU,
                Title = productVariant.DisplayName,
                StandardProductIDType = StandardProductIDType.EAN
            };
        }

    }
}