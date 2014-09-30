using System;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class ProductShippingChecker : IProductShippingChecker
    {
        private readonly CartModel _cartModel;

        public ProductShippingChecker(CartModel cartModel)
        {
            _cartModel = cartModel;
        }

        public bool CanShip(ProductVariant productVariant)
        {
            if (!productVariant.HasRestrictedShipping)
                return true;
            return _cartModel.PotentiallyAvailableShippingMethods.Any(
                method =>
                    productVariant.RestrictedTo.Contains(method.GetType().FullName, StringComparer.OrdinalIgnoreCase));
        }
    }
}