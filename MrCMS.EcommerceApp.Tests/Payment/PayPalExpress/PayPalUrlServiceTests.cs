using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Payment.PayPalExpress
{
    public class PayPalUrlServiceTests : MrCMSTest
    {
        private readonly Site _currentSite;
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;
        private readonly PayPalUrlService _payPalUrlService;
        private readonly SiteSettings _siteSettings;
        private readonly IUniquePageService _uniquePageService;

        public PayPalUrlServiceTests()
        {
            _currentSite = new Site {BaseUrl = "www.example.com"};
            _payPalExpressCheckoutSettings = new PayPalExpressCheckoutSettings();
            _siteSettings = new SiteSettings();
            _uniquePageService = A.Fake<IUniquePageService>();
            _payPalUrlService = new PayPalUrlService(_currentSite, _payPalExpressCheckoutSettings, _siteSettings,
                _uniquePageService);
        }

        [Fact]
        public void PayPalUrlService_GetReturnURL_ShouldBeHTTPIfSiteIsNotLive()
        {
            _siteSettings.SiteIsLive = false;

            string returnUrl = _payPalUrlService.GetReturnURL();

            returnUrl.Should().StartWith("http://");
        }

        [Fact]
        public void PayPalUrlService_GetReturnURL_ShouldBeHTTPSIfSiteIsLive()
        {
            _siteSettings.SiteIsLive = true;

            string returnUrl = _payPalUrlService.GetReturnURL();

            returnUrl.Should().StartWith("https://");
        }

        [Fact]
        public void PayPalUrlService_GetReturnURL_ShouldReturnRedirectHandler()
        {
            string returnUrl = _payPalUrlService.GetReturnURL();

            returnUrl.Should().Be("http://www.example.com/Apps/Ecommerce/PayPalExpressCheckout/ReturnHandler");
        }

        [Fact]
        public void PayPalUrlService_GetCancelURL_ShouldRedirectBackToCartPage()
        {
            var cart = new Cart {UrlSegment = "site-cart"};
            A.CallTo(() => _uniquePageService.GetUniquePage<Cart>()).Returns(cart);

            string cancelUrl = _payPalUrlService.GetCancelURL();

            cancelUrl.Should().Be("http://www.example.com/site-cart");
        }

        [Fact]
        public void PayPalUrlService_GetExpressCheckoutRedirectUrl_ShouldReturnSandboxUrlIfIsNotLive()
        {
            _payPalExpressCheckoutSettings.IsLive = false;

            string expressCheckoutRedirectUrl = _payPalUrlService.GetExpressCheckoutRedirectUrl("token-value");

            expressCheckoutRedirectUrl.Should()
                .Be("https://www.sandbox.paypal.com/webscr?cmd=_express-checkout&token=token-value");
        }

        [Fact]
        public void PayPalUrlService_GetExpressCheckoutRedirectUrl_ShouldReturnLiveUrlIfIsLive()
        {
            _payPalExpressCheckoutSettings.IsLive = true;

            string expressCheckoutRedirectUrl = _payPalUrlService.GetExpressCheckoutRedirectUrl("token-value");

            expressCheckoutRedirectUrl.Should()
                .Be("https://www.paypal.com/webscr?cmd=_express-checkout&token=token-value");
        }
    }
}