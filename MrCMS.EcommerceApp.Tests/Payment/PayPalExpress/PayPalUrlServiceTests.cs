using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;
using MrCMS.Website;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Payment.PayPalExpress
{
    public class PayPalUrlServiceTests : MrCMSTest
    {
        private readonly Site _currentSite;
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;
        private readonly SiteSettings _siteSettings;
        private readonly PayPalUrlService _payPalUrlService;
        private readonly IDocumentService _documentService;

        public PayPalUrlServiceTests()
        {
            _currentSite = new Site { BaseUrl = "www.example.com" };
            _payPalExpressCheckoutSettings = new PayPalExpressCheckoutSettings();
            _siteSettings = new SiteSettings();
            _documentService = A.Fake<IDocumentService>();
            _payPalUrlService = new PayPalUrlService(_documentService, _currentSite, _payPalExpressCheckoutSettings, _siteSettings);
        }

        [Fact]
        public void PayPalUrlService_GetReturnURL_ShouldBeHTTPIfSiteIsNotLive()
        {
            _siteSettings.SiteIsLive = false;

            var returnUrl = _payPalUrlService.GetReturnURL();

            returnUrl.Should().StartWith("http://");
        }

        [Fact]
        public void PayPalUrlService_GetReturnURL_ShouldBeHTTPSIfSiteIsLive()
        {
            _siteSettings.SiteIsLive = true;

            var returnUrl = _payPalUrlService.GetReturnURL();

            returnUrl.Should().StartWith("https://");
        }

        [Fact]
        public void PayPalUrlService_GetReturnURL_ShouldReturnRedirectHandler()
        {
            var returnUrl = _payPalUrlService.GetReturnURL();

            returnUrl.Should().Be("http://www.example.com/Apps/Ecommerce/PayPalExpressCheckout/ReturnHandler");
        }

        [Fact]
        public void PayPalUrlService_GetCancelURL_ShouldRedirectBackToCartPage()
        {
            var cart = new Cart { UrlSegment = "site-cart" };
            A.CallTo(() => _documentService.GetUniquePage<Cart>()).Returns(cart);

            var cancelUrl = _payPalUrlService.GetCancelURL();

            cancelUrl.Should().Be("http://www.example.com/site-cart");
        }

        [Fact]
        public void PayPalUrlService_GetExpressCheckoutRedirectUrl_ShouldReturnSandboxUrlIfIsNotLive()
        {
            _payPalExpressCheckoutSettings.IsLive = false;

            var expressCheckoutRedirectUrl = _payPalUrlService.GetExpressCheckoutRedirectUrl("token-value");

            expressCheckoutRedirectUrl.Should()
                                      .Be("https://www.sandbox.paypal.com/webscr?cmd=_express-checkout&token=token-value");
        }

        [Fact]
        public void PayPalUrlService_GetExpressCheckoutRedirectUrl_ShouldReturnLiveUrlIfIsLive()
        {
            _payPalExpressCheckoutSettings.IsLive = true;

            var expressCheckoutRedirectUrl = _payPalUrlService.GetExpressCheckoutRedirectUrl("token-value");

            expressCheckoutRedirectUrl.Should()
                                      .Be("https://www.paypal.com/webscr?cmd=_express-checkout&token=token-value");
        }
    }
}