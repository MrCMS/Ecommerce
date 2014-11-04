using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class ProductShippingCheckerBuilder
    {
        private CartModel _cartModel = new CartModel();

        public ProductShippingChecker Build()
        {
            return new ProductShippingChecker(_cartModel);
        }

        public ProductShippingCheckerBuilder WithCartModel(CartModel cartModel)
        {
            _cartModel = cartModel;
            return this;
        }
    }
}