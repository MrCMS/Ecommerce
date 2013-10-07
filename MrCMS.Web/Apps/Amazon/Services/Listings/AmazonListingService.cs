using System;
using MarketplaceWebServiceProducts.Model;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api.Products;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Amazon.Services.Listings
{
    public class AmazonListingService : IAmazonListingService
    {
        private readonly ISession _session;
        private readonly IAmazonLogService _amazonLogService;
        private readonly IAmazonProductsApiService _amazonProductsApiService;
        private readonly IProductVariantService _productVariantService;
        private readonly IOptionService _optionService;

        public AmazonListingService(ISession session,
            IAmazonLogService amazonLogService, 
            IProductVariantService productVariantService, IOptionService optionService, IAmazonProductsApiService amazonProductsApiService)
        {
            _session = session;
            _amazonLogService = amazonLogService;
            _productVariantService = productVariantService;
            _optionService = optionService;
            _amazonProductsApiService = amazonProductsApiService;
        }

        public AmazonListing Get(int id)
        {
            return _session.QueryOver<AmazonListing>()
                            .Where(item => item.Id == id).SingleOrDefault();
        }

        public AmazonListing GetByProductVariantSku(string sku)
        {
            return _session.QueryOver<AmazonListing>()
                            .Where(item => item.SellerSKU.IsInsensitiveLike(sku,MatchMode.Exact)).SingleOrDefault();
        }

        public void Save(AmazonListing item)
        {
            var id = item.Id;
            _session.Transact(session => session.SaveOrUpdate(item));
            _amazonLogService.Add(AmazonLogType.Listings, id > 0 ? AmazonLogStatus.Update : AmazonLogStatus.Insert,
                                  null, null, null, null, null, item, null);
        }

        public void Delete(AmazonListing item)
        {
            _amazonLogService.Add(AmazonLogType.Listings, AmazonLogStatus.Delete, null, null, null, null, null, item, null);

            _session.Transact(session => session.Delete(item));
        }

        public void UpdateAmazonListingStatus(AmazonListing item)
        {
            var amazonProduct = _amazonProductsApiService.GetMatchingProductForId(item.SellerSKU);
            if (amazonProduct == null && !String.IsNullOrWhiteSpace(item.ASIN))
                item.Status = AmazonListingStatus.Inactive;
            else
                item.Status = AmazonListingStatus.Active;
            Save(item);
        }

        public void UpdateAmazonListingStatusAndAsin(AmazonListing item, Product amazonProduct)
        {
            if (amazonProduct == null)
                amazonProduct = _amazonProductsApiService.GetMatchingProductForId(item.SellerSKU);

            if (amazonProduct != null && amazonProduct.Identifiers.MarketplaceASIN != null)
            {
                    item.Status = AmazonListingStatus.Active;
                    item.ASIN = amazonProduct.Identifiers.MarketplaceASIN.ASIN;
            }
            else
            {
                item.Status = String.IsNullOrWhiteSpace(item.ASIN)
                                  ? AmazonListingStatus.NotOnAmazon
                                  : AmazonListingStatus.Inactive;
            }
            Save(item);
        }

        public AmazonListingModel GetAmazonListingModel(AmazonListingGroup amazonListingGroup)
        {
            return new AmazonListingModel()
            {
                ProductVariants = _productVariantService.GetAllVariants(String.Empty),
                AmazonListingGroup = amazonListingGroup,
                Categories = _optionService.GetCategoryOptions()
            };
        }

        public AmazonListingModel GetAmazonListingModel(AmazonListingModel oldModel)
        {
            return new AmazonListingModel()
            {
                AmazonListingGroup = oldModel.AmazonListingGroup,
                ProductVariants = _productVariantService.GetAllVariants(oldModel.Name, oldModel.CategoryId, oldModel.Page),
                CategoryId = oldModel.CategoryId,
                Categories = _optionService.GetCategoryOptions(),
                Page = oldModel.Page
            };
        }
    }
}