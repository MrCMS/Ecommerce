using System;
using MarketplaceWebServiceFeedsClasses;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Amazon.Services.Listings
{
    public class AmazonListingGroupService : IAmazonListingGroupService
    {
        private readonly ISession _session;
        private readonly IAmazonListingService _amazonListingService;
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly AmazonSellerSettings _amazonSellerSettings;
        private readonly IProductVariantService _productVariantService;

        public AmazonListingGroupService(ISession session,
            IAmazonListingService amazonListingService, 
            EcommerceSettings ecommerceSettings,
            AmazonSellerSettings amazonSellerSettings, 
            IProductVariantService productVariantService)
        {
            _session = session;
            _amazonListingService = amazonListingService;
            _ecommerceSettings = ecommerceSettings;
            _amazonSellerSettings = amazonSellerSettings;
            _productVariantService = productVariantService;
        }

        public AmazonListingGroup Get(int id)
        {
            return _session.QueryOver<AmazonListingGroup>()
                            .Where(item => item.Id == id).SingleOrDefault();
        }

        public IPagedList<AmazonListingGroup> Search(string queryTerm = null, int page = 1, int pageSize = 10)
        {
            if (!string.IsNullOrWhiteSpace(queryTerm))
            {
                return _session.QueryOver<AmazonListingGroup>()
                                    .Where(x =>x.Name.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)).Paged(page, pageSize);
            }

            return _session.Paged(QueryOver.Of<AmazonListingGroup>(), page, pageSize);
        }

        public AmazonListingGroup Save(AmazonListingGroup item)
        {
            _session.Transact(session => session.SaveOrUpdate(item));
            return item;
        }

        public void Delete(AmazonListingGroup item)
        {
            _session.Transact(session => session.Delete(item));
        }

        public void InitAmazonListingsFromProductVariants(AmazonListingGroup amazonListingGroup, string rawProductVariantsIds)
        {
            try
            {
                var productVariantsIds = rawProductVariantsIds.Trim().Split(',');
                foreach (var item in productVariantsIds)
                {
                    if (String.IsNullOrWhiteSpace(item)) continue;

                    var amazonListing = _amazonListingService.GetByProductVariantSKU(item);

                    if (amazonListing != null) continue;

                    InitAmazonListingFromProductVariant(item, amazonListingGroup.Id);
                }
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
        }

        public AmazonListing InitAmazonListingFromProductVariant(string productVariantSku, int amazonListingGroupId)
        {
            var productVariant = _productVariantService.GetProductVariantBySKU(productVariantSku);
            var amazonListingGroup = Get(amazonListingGroupId);

            var amazonListing = new AmazonListing()
            {
                AmazonListingGroup = amazonListingGroup,
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
                StandardProductIDType = _amazonSellerSettings.BarcodeIsOfType,
                StandardProductId = productVariant.Barcode,
                Status = AmazonListingStatus.Active
            };

            amazonListingGroup.Items.Add(amazonListing);

            Save(amazonListingGroup);

            return amazonListing;
        }
    }
}