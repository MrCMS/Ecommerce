using System;
using MarketplaceWebServiceFeedsClasses;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Pricing;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Services.Listings.Sync
{
    public class PrepareForSyncAmazonListingService : IPrepareForSyncAmazonListingService
    {
        private readonly IAmazonListingService _amazonListingService;
        private readonly IAmazonListingGroupService _amazonListingGroupService;
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly AmazonSellerSettings _amazonSellerSettings;
        private readonly IProductVariantService _productVariantService;
        private readonly IGetStockRemainingQuantity _getStockRemainingQuantity;
        private readonly IProductPricingMethod _productPricingMethod;

        public PrepareForSyncAmazonListingService(
            IAmazonListingService amazonListingService, 
            IAmazonListingGroupService amazonListingGroupService, 
            EcommerceSettings ecommerceSettings,
            AmazonSellerSettings amazonSellerSettings, 
            IProductVariantService productVariantService,
            IGetStockRemainingQuantity getStockRemainingQuantity, 
            IProductPricingMethod productPricingMethod)
        {
            _amazonListingService = amazonListingService;
            _amazonListingGroupService = amazonListingGroupService;
            _ecommerceSettings = ecommerceSettings;
            _amazonSellerSettings = amazonSellerSettings;
            _productVariantService = productVariantService;
            _getStockRemainingQuantity = getStockRemainingQuantity;
            _productPricingMethod = productPricingMethod;
        }

        public void UpdateAmazonListings(AmazonListingGroup amazonListingGroup)
        {
            foreach (var item in amazonListingGroup.Items)
            {
                UpdateAmazonListing(item);
            }
        }

        public AmazonListing UpdateAmazonListing(AmazonListing amazonListing)
        {
            var productVariant = _productVariantService.GetProductVariantBySKU(amazonListing.ProductVariant.SKU);

            amazonListing.ProductVariant = productVariant;
            amazonListing.Brand = productVariant.Product.Brand != null ? productVariant.Product.Brand.Name : String.Empty;
            amazonListing.Condition = ConditionType.New;
            var currency = _ecommerceSettings.Currency();
            amazonListing.Currency = (currency!=null && !String.IsNullOrWhiteSpace(currency.Code))?currency.Code:CurrencyCode.GBP.GetDescription();
            amazonListing.Manafacturer = productVariant.Product.Brand != null ? productVariant.Product.Brand.Name : String.Empty;
            amazonListing.MfrPartNumber = productVariant.ManufacturerPartNumber;
            amazonListing.Quantity = productVariant.TrackingPolicy == TrackingPolicy.Track
                                          ? _getStockRemainingQuantity.Get(productVariant)
                                          : 1000;
            amazonListing.Price = _productPricingMethod.GetUnitPrice(productVariant, 0m, 0m);
            amazonListing.SellerSKU = productVariant.SKU;
            amazonListing.Title = productVariant.DisplayName;
            amazonListing.StandardProductIDType = _amazonSellerSettings.BarcodeIsOfType;
            amazonListing.StandardProductId = productVariant.Barcode.Trim();

            amazonListing.FulfillmentChannel = amazonListing.AmazonListingGroup.FulfillmentChannel ?? AmazonFulfillmentChannel.MFN;

            _amazonListingService.Save(amazonListing);

            return amazonListing;
        }

        public void InitAmazonListingsFromProductVariants(AmazonListingGroup amazonListingGroup, string rawProductVariantsIds)
        {
            try
            {
                var productVariantsIds = rawProductVariantsIds.Trim().Split(',');
                foreach (var item in productVariantsIds)
                {
                    if (String.IsNullOrWhiteSpace(item)) continue;

                    var amazonListing = _amazonListingService.GetByProductVariantSku(item);

                    InitAmazonListingFromProductVariant(amazonListing, item, amazonListingGroup.Id);
                }
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
        }

        public AmazonListing InitAmazonListingFromProductVariant(AmazonListing amazonListing, string productVariantSku,
                                                                 int amazonListingGroupId)
        {
            var productVariant = _productVariantService.GetProductVariantBySKU(productVariantSku);
            var amazonListingGroup = _amazonListingGroupService.Get(amazonListingGroupId);

            if (amazonListing == null)
                amazonListing = new AmazonListing() { Status = AmazonListingStatus.NotOnAmazon };

            amazonListing.AmazonListingGroup = amazonListingGroup;
            amazonListing.ProductVariant = productVariant;
            amazonListing.Brand = productVariant.Product.Brand != null ? productVariant.Product.Brand.Name : String.Empty;
            amazonListing.Condition = ConditionType.New;
            var currency = _ecommerceSettings.Currency();
            amazonListing.Currency = (currency != null && !String.IsNullOrWhiteSpace(currency.Code)) ? currency.Code : CurrencyCode.GBP.GetDescription();
            amazonListing.Manafacturer = productVariant.Product.Brand != null ? productVariant.Product.Brand.Name : String.Empty;
            amazonListing.MfrPartNumber = productVariant.ManufacturerPartNumber;
            amazonListing.Quantity = productVariant.TrackingPolicy == TrackingPolicy.Track
                                          ? _getStockRemainingQuantity.Get(productVariant)
                                          : 1000;
            amazonListing.Price = _productPricingMethod.GetUnitPrice(productVariant, 0m, 0m);
            amazonListing.SellerSKU = productVariant.SKU;
            amazonListing.Title = productVariant.DisplayName;
            amazonListing.StandardProductIDType = _amazonSellerSettings.BarcodeIsOfType;
            amazonListing.StandardProductId = (string.IsNullOrWhiteSpace(productVariant.Barcode) ? "":productVariant.Barcode.Trim());

            amazonListing.FulfillmentChannel = amazonListing.AmazonListingGroup.FulfillmentChannel ?? AmazonFulfillmentChannel.MFN;

            amazonListingGroup.Items.Add(amazonListing);

            _amazonListingGroupService.Save(amazonListingGroup);

            return amazonListing;
        }
    }
}