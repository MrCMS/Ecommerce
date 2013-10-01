using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Services.Listings;
using MrCMS.Web.Apps.Amazon.Services.Listings.Sync;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Services.Listings.Sync
{
    public class PrepareForSyncAmazonListingServiceTests : InMemoryDatabaseTest
    {
        private IAmazonListingService _amazonListingService;
        private IAmazonListingGroupService _amazonListingGroupService;
        private EcommerceSettings _ecommerceSettings;
        private AmazonSellerSettings _amazonSellerSettings;
        private IProductVariantService _productVariantService;
        private PrepareForSyncAmazonListingService _prepareForSyncAmazonListingService;

        public PrepareForSyncAmazonListingServiceTests()
        {
            _amazonListingService = A.Fake<IAmazonListingService>();
            _amazonListingGroupService = A.Fake<IAmazonListingGroupService>();
            _ecommerceSettings = A.Fake<EcommerceSettings>();
            _amazonSellerSettings = A.Fake<AmazonSellerSettings>();
            _productVariantService = A.Fake<IProductVariantService>();
            _prepareForSyncAmazonListingService = new PrepareForSyncAmazonListingService(_amazonListingService,_amazonListingGroupService,
            _ecommerceSettings,_amazonSellerSettings,_productVariantService);
        }


        [Fact]
        public void PrepareForSyncAmazonListingService_UpdateAmazonListing_ShouldReturnAmazonListingType()
        {
            var product = new Product();
            var productVariant = new ProductVariant() { Product = product, SKU = "S1"};
            var model = new AmazonListing() {  };
            //var p = new Product()
            //{
            //    Name = "P"
            //};
            //var pv = new ProductVariant()
            //    {
            //        SKU = "S1",
                   
            //    };
            //var model = new AmazonListing()
            //    {
            //       ProductVariant = pv
            //    };

            ////A.CallTo(() => _productVariantService.GetProductVariantBySKU(model.ProductVariant.SKU)).Returns(pv);

            //var results = _prepareForSyncAmazonListingService.UpdateAmazonListing(model);

            //results.Should().BeOfType<AmazonListing>();
        }

        [Fact]
        public void PrepareForSyncAmazonListingService_InitAmazonListingFromProductVariant_ShouldReturnAmazonListingType()
        {
            var model = new AmazonListing();

            var results = _prepareForSyncAmazonListingService.InitAmazonListingFromProductVariant(model,"S1",1);

            results.Should().BeOfType<AmazonListing>();
        }
    }
}