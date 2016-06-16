using System.Collections.Generic;
using FakeItEasy;
using MrCMS.EcommerceApp.Tests.TestableModels;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class CartModelBuilder
    {
        private decimal? _totalPreShipping;
        private decimal? _weight;
        private string _shippingAddressCountry;
        private readonly List<CartItemData> _items = new List<CartItemData>();
       

        private decimal? _shippableCalculationTotal;
        private IShippingMethod[] _methods = new IShippingMethod[1];

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

        public CartModelBuilder WithShippableCalculationTotal(decimal shippableCalculationTotal)
        {
            _shippableCalculationTotal = shippableCalculationTotal;
            return this;
        }

        public CartModelBuilder WithShippingAddressCountry(string code)
        {
            _shippingAddressCountry = code;
            return this;
        }

        public CartModelBuilder WithItems(params CartItemData[] items)
        {
            _items.Clear();
            items.ForEach(info => _items.Add(info));
            return this;
        }

        public CartModel Build()
        {
            return new TestableCartModel(_weight, _totalPreShipping)
                       {
                           ShippingAddress = new Address { CountryCode = _shippingAddressCountry },
                           Items = _items,
                           PotentiallyAvailableShippingMethods = _methods.ToHashSet()
                       };
        }

        public CartModelBuilder WithShippingOptions(params IShippingMethod[] methods)
        {
            _methods = methods;
            return this;
        }
    }
}