using MrCMS.EcommerceApp.Tests.TestableModels;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public class CartModelBuilder
    {
        private decimal? _totalPreShipping;
        private decimal? _weight;
        private Country _shippingAddressCountry;

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

        public CartModel Build()
        {
            return new TestableCartModel(_weight, _totalPreShipping)
                       {
                           ShippingAddress = new Address
                                                 {
                                                     Country = _shippingAddressCountry
                                                 }
                       };
        }
    }
}