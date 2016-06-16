using FakeItEasy;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Services.Pricing;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class GoogleBaseManagerTests : InMemoryDatabaseTest
    {
        private readonly GoogleBaseManager _googleBaseManager;
        private readonly IProductPricingMethod _productPricingMethod = A.Fake<IProductPricingMethod>();
        private readonly IGetStockRemainingQuantity _getStockRemainingQuantity;

        private readonly IGoogleBaseShippingService _googleBaseShippingService;

        public GoogleBaseManagerTests()
        {
            _googleBaseShippingService = A.Fake<IGoogleBaseShippingService>();
            _getStockRemainingQuantity = A.Fake<IGetStockRemainingQuantity>();
            _googleBaseManager = new GoogleBaseManager(Session, _googleBaseShippingService, _getStockRemainingQuantity,
                _productPricingMethod);
        }

        [Fact]
        public void GoogleBaseManager_ExportProductsToGoogleBase_ShouldNotBeNull()
        {
            var result = _googleBaseManager.ExportProductsToGoogleBase();

            result.Should().NotBeNull();
        }

        [Fact]
        public void GoogleBaseManager_ExportProductsToGoogleBase_ShouldReturnByteArray()
        {
            var result = _googleBaseManager.ExportProductsToGoogleBase();

            result.Should().BeOfType<byte[]>();
        }

        [Fact]
        public void GoogleBaseService_SaveGoogleBaseProduct_ShouldUpdateItem()
        {
            var pv = new ProductVariant();
            var item = new GoogleBaseProduct {ProductVariant = pv};

            Session.Transact(session => session.Save(item));
            item.Grouping = "Group2";

            _googleBaseManager.SaveGoogleBaseProduct(item);
            Session.Evict(item);

            Session.QueryOver<GoogleBaseProduct>().SingleOrDefault().Grouping.Should().Be("Group2");
        }
    }
}