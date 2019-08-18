using FakeItEasy;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api.Products;
using MrCMS.Web.Apps.Amazon.Services.Listings;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;

namespace MrCMS.AmazonApp.Tests.Services.Listings
{
    public class AmazonListingServiceTests : InMemoryDatabaseTest
    {
       private readonly IAmazonLogService _amazonLogService;
        private readonly IAmazonProductsApiService _amazonProductsApiService;
        private readonly IProductVariantService _productVariantService;
        private readonly IOptionService _optionService;
        private AmazonListingService _amazonListingService;

        public AmazonListingServiceTests()
        {
            _amazonLogService = A.Fake<IAmazonLogService>();
            _amazonProductsApiService = A.Fake<IAmazonProductsApiService>();
            _productVariantService = A.Fake<IProductVariantService>();
            _optionService = A.Fake<IOptionService>();
            _amazonListingService = new AmazonListingService(Session,_amazonLogService,_productVariantService,_optionService,_amazonProductsApiService);
        }

        [Fact]
        public void AmazonListingService_Get_ShouldReturnPersistedEntryFromSession()
        {
            var item = new AmazonListing();
            Session.Transact(session => session.Save(item));

            var results=_amazonListingService.Get(1);

            results.As<AmazonListing>().Id.Should().Be(1);
        }

        [Fact]
        public void AmazonListingService_GetByProductVariantSku_ShouldReturnPersistedEntryFromSession()
        {
            var item = new AmazonListing() { SellerSKU = "T1"};
            Session.Transact(session => session.Save(item));

            var results = _amazonListingService.GetByProductVariantSku("T1");

            results.As<AmazonListing>().Id.Should().Be(1);
        }

        [Fact]
        public void AmazonListingService_Save_ShouldUpdateInSession()
        {
            var item = new AmazonListing();
            Session.Transact(session => session.Save(item));
            item.SellerSKU = "T1";

            _amazonListingService.Save(item);
            Session.Evict(item);

            Session.Get<AmazonListing>(1).SellerSKU.Should().Be("T1");
        }

        [Fact]
        public void AmazonListingService_Save_ShouldCallAddLog()
        {
            var item = new AmazonListing();

            _amazonListingService.Save(item);

            A.CallTo(() => _amazonLogService.Add(AmazonLogType.Listings, AmazonLogStatus.Insert,
                                  null, null, null, null, null, item, null, string.Empty, string.Empty)).MustHaveHappened();
        }

        [Fact]
        public void AmazonListingService_Delete_ShouldRemoveItemFromTheSession()
        {
            var item = new AmazonListing();
            Session.Transact(session => session.Save(item));

            _amazonListingService.Delete(item);

            Session.QueryOver<AmazonListing>().RowCount().Should().Be(0);
        }

        [Fact]
        public void AmazonListingService_Delete_ShouldCallAddLog()
        {
            var item = new AmazonListing();

            _amazonListingService.Delete(item);

            A.CallTo(() => _amazonLogService.Add(AmazonLogType.Listings, AmazonLogStatus.Delete, null, null, null, null, null, item, null, string.Empty, string.Empty)).MustHaveHappened();
        }

        [Fact]
        public void AmazonListingService_GetAmazonListingModel_ShouldReturnAmazonListingModelType()
        {
            var model = new AmazonListingGroup();

            var results = _amazonListingService.GetAmazonListingModel(model);

            results.Should().BeOfType<AmazonListingModel>();
        }

        [Fact]
        public void AmazonListingService_GetAmazonListingModel_ShouldReturnAmazonListingModelTypeSecondMethod()
        {
            var model = new AmazonListingModel();

            var results = _amazonListingService.GetAmazonListingModel(model);

            results.Should().BeOfType<AmazonListingModel>();
        }
    }
}