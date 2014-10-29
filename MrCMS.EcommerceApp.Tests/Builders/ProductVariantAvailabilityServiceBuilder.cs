using FakeItEasy;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class ProductVariantAvailabilityServiceBuilder
    {
        private readonly IProductShippingChecker _productShippingChecker;
        private readonly IProductStockChecker _productStockChecker;
        private readonly CanOrderQuantityResult _canOrderQuantityResult = new CanOrderQuantityResult{CanOrder = true};

        public ProductVariantAvailabilityServiceBuilder()
        {
            _productStockChecker = A.Fake<IProductStockChecker>();
            A.CallTo(() => _productStockChecker.IsInStock(A<ProductVariant>._)).Returns(true);
            A.CallTo(() => _productStockChecker.CanOrderQuantity(A<ProductVariant>._, A<int>._)).Returns(_canOrderQuantityResult);
            _productShippingChecker = A.Fake<IProductShippingChecker>();
            A.CallTo(() => _productShippingChecker.CanShip(A<ProductVariant>._)).Returns(true);
        }

        public ProductVariantAvailabilityService Build()
        {
            return new ProductVariantAvailabilityService(_productStockChecker, _productShippingChecker);
        }

        public ProductVariantAvailabilityServiceBuilder IsOutOfStock()
        {
            A.CallTo(() => _productStockChecker.IsInStock(A<ProductVariant>._)).Returns(false);
            return this;
        }

        public ProductVariantAvailabilityServiceBuilder CannotOrderQuantity()
        {
            _canOrderQuantityResult.CanOrder = false;
            return this;
        }

        public ProductVariantAvailabilityServiceBuilder CannotShip()
        {
            A.CallTo(() => _productShippingChecker.CanShip(A<ProductVariant>._)).Returns(false);
            return this;
        }
    }
}