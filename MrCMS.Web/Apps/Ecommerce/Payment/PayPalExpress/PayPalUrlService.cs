using System;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PayPalUrlService : IPayPalUrlService
    {
        private readonly IDocumentService _documentService;
        private readonly Site _currentSite;
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;
        private readonly SiteSettings _siteSettings;

        public PayPalUrlService(IDocumentService documentService, Site currentSite, PayPalExpressCheckoutSettings payPalExpressCheckoutSettings, SiteSettings siteSettings)
        {
            _documentService = documentService;
            _currentSite = currentSite;
            _payPalExpressCheckoutSettings = payPalExpressCheckoutSettings;
            _siteSettings = siteSettings;
        }

        private string GetScheme()
        {
            return _siteSettings.SiteIsLive ? "https://" : "http://";
        }

        public string GetReturnURL()
        {
            return
                new Uri(new Uri(GetScheme() + _currentSite.BaseUrl), "Apps/Ecommerce/PayPalExpressCheckout/ReturnHandler")
                    .ToString();
        }

        public string GetCancelURL()
        {
            var cart = _documentService.GetUniquePage<Cart>();
            return
                new Uri(new Uri(GetScheme() + _currentSite.BaseUrl), cart == null ? string.Empty : cart.LiveUrlSegment)
                    .ToString();
        }

        public string GetExpressCheckoutRedirectUrl(string token)
        {
            return
                string.Format(
                    _payPalExpressCheckoutSettings.IsLive
                        ? "https://www.paypal.com/webscr?cmd=_express-checkout&token={0}"
                        : "https://www.sandbox.paypal.com/webscr?cmd=_express-checkout&token={0}", token);
        }
    }
}