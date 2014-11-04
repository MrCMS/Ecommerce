using System;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.TestableModels;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Payment.PayPalExpress
{
    public class PayPalShippingServiceTests
    {
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;
        private readonly PayPalShippingService _payPalShippingService;

        public PayPalShippingServiceTests()
        {
            _payPalExpressCheckoutSettings = new PayPalExpressCheckoutSettings();
            _payPalShippingService = new PayPalShippingService(_payPalExpressCheckoutSettings);
        }

        [Fact]
        public void PayPalShippingServiceTests_GetRequireConfirmedShippingAddress_ShouldBe1IfSettingIsTrue()
        {
            _payPalExpressCheckoutSettings.RequireConfirmedShippingAddress = true;

            _payPalShippingService.GetRequireConfirmedShippingAddress().Should().Be("1");
        }

        [Fact]
        public void PayPalShippingServiceTests_GetRequireConfirmedShippingAddress_ShouldBe0IfSettingIsFalse()
        {
            _payPalExpressCheckoutSettings.RequireConfirmedShippingAddress = false;

            _payPalShippingService.GetRequireConfirmedShippingAddress().Should().Be("0");
        }

        [Fact]
        public void PayPalShippingServiceTests_GetNoShipping_ShouldBe2WhenRequiresShipping()
        {
            _payPalShippingService.GetNoShipping(new TestableCartModel(requiresShipping: true)).Should().Be("2");
        }

        [Fact]
        public void PayPalShippingServiceTests_GetNoShipping_ShouldBe0WhenRequiresShippingAndAddressIsSet()
        {
            var testableCartModel = new TestableCartModel(requiresShipping: true) {ShippingAddress = new Address()};
            _payPalShippingService.GetNoShipping(testableCartModel).Should().Be("0");
        }

        [Fact]
        public void PayPalShippingServiceTests_GetNoShipping_ShouldBe1WhenShippingNotRequired()
        {
            _payPalShippingService.GetNoShipping(new TestableCartModel(requiresShipping: false)).Should().Be("1");
        }
    }
}