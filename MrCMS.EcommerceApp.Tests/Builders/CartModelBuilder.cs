using System.Collections.Generic;
using MrCMS.EcommerceApp.Tests.TestableModels;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Helpers;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class CartModelBuilder
    {
        private decimal? _totalPreShipping;
        private decimal? _weight;
        private Country _shippingAddressCountry;
        private readonly List<CartItem> _items = new List<CartItem>();
        private readonly IList<ShippingMethod> _availableShippingMethods = new List<ShippingMethod>
                                                                            {
                                                                                new TestableShippingMethod(true)
                                                                            };

        public CartModelBuilder WithWeight(decimal weight)
        {
            _weight = weight;
            return this;
        }

        public CartModelBuilder WithTotalPreShipping(decimal totalPreShipping)
        {
            _totalPreShipping = totalPreShipping;
            return this;
        }

        public CartModelBuilder WithShippingAddressCountry(Country country)
        {
            _shippingAddressCountry = country;
            return this;
        }

        public CartModelBuilder WithItems(params CartItem[] items)
        {
            _items.Clear();
            items.ForEach(info => _items.Add(info));
            return this;
        }

        public CartModel Build()
        {
            return new TestableCartModel(_weight, _totalPreShipping)
                       {
                           ShippingAddress = new Address { Country = _shippingAddressCountry },
                           Items = _items,
                           AvailableShippingMethods = _availableShippingMethods
                       };
        }
    }
}