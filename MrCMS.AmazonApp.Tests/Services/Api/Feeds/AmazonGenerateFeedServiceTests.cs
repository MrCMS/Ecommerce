using FakeItEasy;
using FluentAssertions;
using MarketplaceWebServiceFeedsClasses;
using MrCMS.EcommerceApp.Tests;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Services.Api.Feeds;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using NHibernate;
using Ninject.MockingKernel;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Services.Api.Feeds
{
    public class AmazonGenerateFeedServiceTests : InMemoryDatabaseTest
    {
        private AmazonSellerSettings _amazonSellerSettings;
        private AmazonGenerateFeedService _amazonGenerateFeedService;
        private EcommerceSettings _ecommerceSettings;

        public AmazonGenerateFeedServiceTests()
        {
            _amazonSellerSettings = A.Fake<AmazonSellerSettings>();
            _ecommerceSettings = A.Fake<EcommerceSettings>();
            _amazonGenerateFeedService = new AmazonGenerateFeedService(_amazonSellerSettings, _ecommerceSettings);
        }

        [Fact]
        public void AmazonGenerateFeedService_GetProduct_ShouldReturnProductType()
        {
            var model = new AmazonListing()
                {
                    SellerSKU = "S1"
                };
            var results = _amazonGenerateFeedService.GetProduct(model);

            results.Should().BeOfType<Product>();
        }

        [Fact]
        public void AmazonGenerateFeedService_GetProduct_ShouldSetValues()
        {
            var listing = new AmazonListing()
            {
                SellerSKU = "S1",
                Manafacturer = "M",
                Brand = "B",
                StandardProductId = "SP",
                StandardProductIDType = StandardProductIDType.EAN,
                Condition = ConditionType.New,
                ConditionNote = "CN",
                Title = "T",
                ReleaseDate = CurrentRequestData.Now,
                MfrPartNumber = "MPN1"
            };
            var results = _amazonGenerateFeedService.GetProduct(listing);

            results.As<Product>().Condition.ConditionType.Should().Be(listing.Condition);
            results.As<Product>().Condition.ConditionNote.Should().Be(listing.ConditionNote);
            results.As<Product>().SKU.Should().Be(listing.SellerSKU);
            results.As<Product>().StandardProductID.Type.Should().Be(listing.StandardProductIDType);
            results.As<Product>().StandardProductID.Value.Should().Be(listing.StandardProductId);
            results.As<Product>().DescriptionData.Brand.Should().Be(listing.Brand);
            results.As<Product>().DescriptionData.Title.Should().Be(listing.Title);
            results.As<Product>().DescriptionData.Manufacturer.Should().Be(listing.Manafacturer);
            results.As<Product>().DescriptionData.MfrPartNumber.Should().Be(listing.MfrPartNumber);
            results.As<Product>().ReleaseDate.Should().Be(listing.ReleaseDate.Value);
        }

        [Fact]
        public void AmazonGenerateFeedService_GetProductPrice_ShouldReturnPriceType()
        {
            var mockingKernel = new MockingKernel();
            MrCMSApplication.OverrideKernel(mockingKernel);
            mockingKernel.Bind<ISession>().ToMethod(context => A.Fake<ISession>());

            var model = new AmazonListing()
            {
                SellerSKU = "S1"
            };
            var results = _amazonGenerateFeedService.GetProductPrice(model);

            results.Should().BeOfType<Price>();
        }

        [Fact]
        public void AmazonGenerateFeedService_GetProductPrice_ShouldSetValues()
        {
            var mockingKernel = new MockingKernel();
            MrCMSApplication.OverrideKernel(mockingKernel);
            mockingKernel.Bind<ISession>().ToMethod(context => A.Fake<ISession>());

            var listing = new AmazonListing()
            {
                SellerSKU = "S1",
                Price = 1
            };
            var results = _amazonGenerateFeedService.GetProductPrice(listing);

            results.As<Price>().StandardPrice.Value.Should().Be(listing.Price);
            results.As<Price>().StandardPrice.Currency.Should().Be(BaseCurrencyCodeWithDefault.USD);
            results.As<Price>().SKU.Should().Be(listing.SellerSKU);
        }

        [Fact]
        public void AmazonGenerateFeedService_GetProductInventory_ShouldReturnPriceType()
        {
            var model = new AmazonListing()
            {
                SellerSKU = "S1"
            };
            var results = _amazonGenerateFeedService.GetProductInventory(model);

            results.Should().BeOfType<Inventory>();
        }

        [Fact]
        public void AmazonGenerateFeedService_GetProductInventory_ShouldSetValues()
        {
            var listing = new AmazonListing()
            {
                SellerSKU = "S1",
                Quantity = 1
            };
            var results = _amazonGenerateFeedService.GetProductInventory(listing);

            results.As<Inventory>().SwitchFulfillmentTo.Should().Be(InventorySwitchFulfillmentTo.MFN);
            results.As<Inventory>().FulfillmentCenterID.Should().Be("DEFAULT");
            results.As<Inventory>().Item.Should().Be(listing.Quantity.ToString());
            results.As<Inventory>().SKU.Should().Be(listing.SellerSKU);
        }

        [Fact]
        public void AmazonGenerateFeedService_GetProductImage_ShouldReturnNullIfProductVariantNull()
        {
            var model = new AmazonListing()
            {
                SellerSKU = "S1"
            };
            var results = _amazonGenerateFeedService.GetProductImage(model);

            results.Should().BeNull();
        }
    }
}