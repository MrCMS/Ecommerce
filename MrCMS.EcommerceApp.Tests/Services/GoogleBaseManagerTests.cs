using FakeItEasy;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;
using MrCMS.Web.Apps.Ecommerce.Services.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class GoogleBaseManagerTests :InMemoryDatabaseTest
    {
        private readonly GoogleBaseManager _googleBaseManager;
        private readonly IProductVariantService _productVariantService;
        private readonly IOrderShippingService _orderShippingService;

        public GoogleBaseManagerTests()
        {
            _productVariantService = A.Fake<IProductVariantService>();
            _orderShippingService = A.Fake<IOrderShippingService>();
            _googleBaseManager = new GoogleBaseManager(Session, _productVariantService, _orderShippingService);
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
        public void GoogleBaseManager_ExportProductsToGoogleBase_ShouldCallGetAllVariantsOfProductVariantService()
        {
            _googleBaseManager.ExportProductsToGoogleBase();

            A.CallTo(() => _productVariantService.GetAllVariantsForGoogleBase()).MustHaveHappened();
        }

        [Fact]
        public void GoogleBaseService_SaveGoogleBaseProduct_ShouldUpdateItem()
        {
            var pv = new ProductVariant();
            var item = new GoogleBaseProduct { ProductVariant = pv };

            Session.Transact(session => session.Save(item));
            item.Grouping = "Group2";

            _googleBaseManager.SaveGoogleBaseProduct(item);
            Session.Evict(item);

            Session.QueryOver<GoogleBaseProduct>().SingleOrDefault().Grouping.Should().Be("Group2");
        }
    }
}
