using FakeItEasy;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class ProductStockCheckerBuilder
    {
        private readonly IGetStockRemainingQuantity _getStockRemainingQuantity = A.Fake<IGetStockRemainingQuantity>();

        public ProductStockChecker Build()
        {
            return new ProductStockChecker(_getStockRemainingQuantity);
        }

        public ProductStockCheckerBuilder StockRemaining(int remaining)
        {
            A.CallTo(() => _getStockRemainingQuantity.Get(A<ProductVariant>._)).Returns(remaining);
            return this;
        }
    }
}