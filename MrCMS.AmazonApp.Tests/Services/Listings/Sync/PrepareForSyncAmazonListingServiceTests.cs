using FakeItEasy;
using FluentAssertions;
using MarketplaceWebServiceFeedsClasses;
using MrCMS.EcommerceApp.Tests;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Listings;
using MrCMS.Web.Apps.Amazon.Services.Listings.Sync;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Currencies;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using NHibernate;
using Ninject.MockingKernel;
using Xunit;
using Product = MrCMS.Web.Apps.Ecommerce.Pages.Product;

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
            var mockingKernel = new MockingKernel();
            MrCMSApplication.OverrideKernel(mockingKernel);
            mockingKernel.Bind<ISession>().ToMethod(context => A.Fake<ISession>());

            var product = new Product();
            var productVariant = new ProductVariant() { Product = product, SKU = "S1", Barcode = "" };
            var model = new AmazonListing() { ProductVariant = productVariant, AmazonListingGroup = new AmazonListingGroup()
                {
                    FulfillmentChannel = AmazonFulfillmentChannel.MFN
                }};

            A.CallTo(() => _productVariantService.GetProductVariantBySKU(model.ProductVariant.SKU)).Returns(productVariant);

            var results = _prepareForSyncAmazonListingService.UpdateAmazonListing(model);

            results.Should().BeOfType<AmazonListing>();
        }

        [Fact]
        public void PrepareForSyncAmazonListingService_UpdateAmazonListing_ShouldSetValues()
        {
            var item = new Currency() { Code = "GBP", Id=1 };
            Session.Transact(session => session.Save(item));

            var mockingKernel = new MockingKernel();
            MrCMSApplication.OverrideKernel(mockingKernel);
            mockingKernel.Bind<ISession>().ToMethod(context => A.Fake<ISession>());
            mockingKernel.Bind<EcommerceSettings>().ToMethod(context => new EcommerceSettings(){CurrencyId = 1});

            var product = new Product()
                {
                    Brand = new Brand(){Name = "B"}
                };
            var productVariant = new ProductVariant()
                {
                    Product = product, 
                    SKU = "S1",
                    BasePrice = 1,
                    StockRemaining = 2,
                    Name = "P",
                    ManufacturerPartNumber = "MPN1",
                    Barcode = ""
                };
            var model = new AmazonListing()
            {
                ProductVariant = productVariant,
                StandardProductId = "1P",
                StandardProductIDType = StandardProductIDType.EAN,
                AmazonListingGroup = new AmazonListingGroup()
                {
                    FulfillmentChannel = AmazonFulfillmentChannel.MFN
                }
            };

            A.CallTo(() => _productVariantService.GetProductVariantBySKU(model.ProductVariant.SKU)).Returns(productVariant);

            var results = _prepareForSyncAmazonListingService.UpdateAmazonListing(model);

            results.As<AmazonListing>().Condition.Should().Be(ConditionType.New);
            results.As<AmazonListing>().Currency.Should().Be("GBP");
            results.As<AmazonListing>().Manafacturer.Should().Be("B");
            results.As<AmazonListing>().Brand.Should().Be("B");
            results.As<AmazonListing>().MfrPartNumber.Should().Be("MPN1");
            results.As<AmazonListing>().Price.Should().Be(1);
            results.As<AmazonListing>().Quantity.Should().Be(2);
            results.As<AmazonListing>().Title.Should().Be("P");
            results.As<AmazonListing>().StandardProductIDType.Should().Be(_amazonSellerSettings.BarcodeIsOfType);
            results.As<AmazonListing>().StandardProductId.Should().Be(model.StandardProductId);
            results.As<AmazonListing>().FulfillmentChannel.Should().Be(AmazonFulfillmentChannel.MFN);
        }

        [Fact]
        public void PrepareForSyncAmazonListingService_UpdateAmazonListing_ShouldCallSave()
        {
            var item = new Currency() { Code = "GBP", Id = 1 };
            Session.Transact(session => session.Save(item));

            var mockingKernel = new MockingKernel();
            MrCMSApplication.OverrideKernel(mockingKernel);
            mockingKernel.Bind<ISession>().ToMethod(context => A.Fake<ISession>());
            mockingKernel.Bind<EcommerceSettings>().ToMethod(context => new EcommerceSettings() { CurrencyId = 1 });

            var product = new Product()
            {
                Brand = new Brand() { Name = "B" }
            };
            var productVariant = new ProductVariant()
            {
                Product = product,
                SKU = "S1",
                BasePrice = 1,
                StockRemaining = 2,
                Name = "P",
                ManufacturerPartNumber = "MPN1",
                Barcode = ""
            };
            var model = new AmazonListing()
            {
                ProductVariant = productVariant,
                StandardProductId = "1P",
                StandardProductIDType = StandardProductIDType.EAN,
                AmazonListingGroup = new AmazonListingGroup()
                {
                    FulfillmentChannel = AmazonFulfillmentChannel.MFN
                }
            };

            A.CallTo(() => _productVariantService.GetProductVariantBySKU(model.ProductVariant.SKU)).Returns(productVariant);

            var results = _prepareForSyncAmazonListingService.UpdateAmazonListing(model);

            A.CallTo(() => _amazonListingService.Save(model)).MustHaveHappened();
        }

        [Fact]
        public void PrepareForSyncAmazonListingService_InitAmazonListingFromProductVariant_ShouldReturnAmazonListingType()
        {
            var mockingKernel = new MockingKernel();
            MrCMSApplication.OverrideKernel(mockingKernel);
            mockingKernel.Bind<ISession>().ToMethod(context => A.Fake<ISession>());

            var model = new AmazonListing();

            var results = _prepareForSyncAmazonListingService.InitAmazonListingFromProductVariant(model,"S1",1);

            results.Should().BeOfType<AmazonListing>();
        }

        [Fact]
        public void PrepareForSyncAmazonListingService_InitAmazonListingFromProductVariant_ShouldSetValues()
        {
            var item = new Currency() { Code = "GBP", Id = 1 };
            Session.Transact(session => session.Save(item));

            var mockingKernel = new MockingKernel();
            MrCMSApplication.OverrideKernel(mockingKernel);
            mockingKernel.Bind<ISession>().ToMethod(context => A.Fake<ISession>());
            mockingKernel.Bind<EcommerceSettings>().ToMethod(context => new EcommerceSettings() { CurrencyId = 1 });

            var product = new Product()
            {
                Brand = new Brand() { Name = "B" }
            };
            var productVariant = new ProductVariant()
            {
                Product = product,
                SKU = "S1",
                BasePrice = 1,
                StockRemaining = 2,
                Name = "P",
                ManufacturerPartNumber = "MPN1",
                Barcode = ""
            };
            var amazonListingGroup = new AmazonListingGroup()
                {
                    Id=1,
                    FulfillmentChannel = AmazonFulfillmentChannel.MFN
                };
            var model = new AmazonListing()
            {
                ProductVariant = productVariant,
                StandardProductId = "1P",
                StandardProductIDType = StandardProductIDType.EAN,
                AmazonListingGroup = amazonListingGroup
            };

            A.CallTo(() => _amazonListingGroupService.Get(amazonListingGroup.Id)).Returns(amazonListingGroup);

            A.CallTo(() => _productVariantService.GetProductVariantBySKU(model.ProductVariant.SKU)).Returns(productVariant);

            var results = _prepareForSyncAmazonListingService.InitAmazonListingFromProductVariant(model, model.ProductVariant.SKU, amazonListingGroup.Id);

            results.As<AmazonListing>().Condition.Should().Be(ConditionType.New);
            results.As<AmazonListing>().Currency.Should().Be("GBP");
            results.As<AmazonListing>().Manafacturer.Should().Be("B");
            results.As<AmazonListing>().Brand.Should().Be("B");
            results.As<AmazonListing>().MfrPartNumber.Should().Be("MPN1");
            results.As<AmazonListing>().Price.Should().Be(1);
            results.As<AmazonListing>().Quantity.Should().Be(2);
            results.As<AmazonListing>().Title.Should().Be("P");
            results.As<AmazonListing>().StandardProductIDType.Should().Be(_amazonSellerSettings.BarcodeIsOfType);
            results.As<AmazonListing>().StandardProductId.Should().Be(model.StandardProductId);
            results.As<AmazonListing>().FulfillmentChannel.Should().Be(AmazonFulfillmentChannel.MFN);
        }

        [Fact]
        public void PrepareForSyncAmazonListingService_InitAmazonListingFromProductVariant_ShouldCallSave()
        {
            var item = new Currency() { Code = "GBP", Id = 1 };
            Session.Transact(session => session.Save(item));

            var mockingKernel = new MockingKernel();
            MrCMSApplication.OverrideKernel(mockingKernel);
            mockingKernel.Bind<ISession>().ToMethod(context => A.Fake<ISession>());
            mockingKernel.Bind<EcommerceSettings>().ToMethod(context => new EcommerceSettings() { CurrencyId = 1 });

            var product = new Product()
            {
                Brand = new Brand() { Name = "B" }
            };
            var productVariant = new ProductVariant()
            {
                Product = product,
                SKU = "S1",
                BasePrice = 1,
                StockRemaining = 2,
                Name = "P",
                ManufacturerPartNumber = "MPN1",
                Barcode = ""
            };
            var amazonListingGroup = new AmazonListingGroup()
            {
                Id = 1,
                FulfillmentChannel = AmazonFulfillmentChannel.MFN
            };
            var model = new AmazonListing()
            {
                ProductVariant = productVariant,
                StandardProductId = "1P",
                StandardProductIDType = StandardProductIDType.EAN,
                AmazonListingGroup = amazonListingGroup
            };

            A.CallTo(() => _amazonListingGroupService.Get(amazonListingGroup.Id)).Returns(amazonListingGroup);

            A.CallTo(() => _productVariantService.GetProductVariantBySKU(model.ProductVariant.SKU)).Returns(productVariant);

            var results = _prepareForSyncAmazonListingService.InitAmazonListingFromProductVariant(model, model.ProductVariant.SKU, amazonListingGroup.Id);

            A.CallTo(() =>  _amazonListingGroupService.Save(amazonListingGroup)).MustHaveHappened();
        }
    }
}