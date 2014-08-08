using System;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class ProductVariantAvailabilityService : IProductVariantAvailabilityService
    {
        private readonly CartModel _cart;
        private readonly ISession _session;

        public ProductVariantAvailabilityService(CartModel cart, ISession session)
        {
            _cart = cart;
            _session = session;
        }

        public CanBuyStatus CanBuy(ProductVariant productVariant, int additionalQuantity = 0)
        {
            if (!productVariant.InStock)
                return new OutOfStock(productVariant);
            var requestedQuantity = GetRequestedQuantity(productVariant, additionalQuantity);
            if (productVariant.TrackingPolicy == TrackingPolicy.Track && requestedQuantity > productVariant.StockRemaining)
                return new CannotOrderQuantity(productVariant, requestedQuantity);
            throw new NotImplementedException();
            //var restrictedShippingMethods = _session.QueryOver<ShippingMethod>().JoinQueryOver<ProductVariant>(p => p.ExcludedProductVariants)
            //        .Where(c => c.Id == productVariant.Id).Cacheable().List();
            //if (!_cart.AvailableShippingMethods.Except(restrictedShippingMethods).Any())
            //    return new NoShippingMethodWouldBeAvailable(productVariant);
            //return new CanBuy();
        }

        private int GetRequestedQuantity(ProductVariant productVariant, int additionalQuantity)
        {
            var requestedQuantity = additionalQuantity;
            var existingItem = _cart.Items.FirstOrDefault(item => item.Item == productVariant);
            if (existingItem != null)
                requestedQuantity += existingItem.Quantity;
            return requestedQuantity;
        }
    }
}