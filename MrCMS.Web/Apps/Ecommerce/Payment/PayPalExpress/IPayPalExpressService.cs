using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Logging;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;
using PayPal;
using PayPal.Exception;
using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;
using PayPal.SOAP;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public interface IPayPalExpressService
    {
        SetExpressCheckoutResponse GetSetExpressCheckoutRedirectUrl(CartModel cart);
    }

    public static class PayPalResponseHelper
    {
        public static T2 HandleResponse<T1, T2>(this T1 apiResponse, Action<T1, T2> onSuccess, Action<T1, T2> onFailure)
            where T2 : PayPalResponse, new()
            where T1 : AbstractResponseType
        {
            var mrCMSResponse = new T2();
            switch (apiResponse.Ack)
            {
                case AckCodeType.SUCCESS:
                case AckCodeType.SUCCESSWITHWARNING:
                    onSuccess(apiResponse, mrCMSResponse);
                    break;
                default:
                    onFailure(apiResponse, mrCMSResponse);
                    break;
            }
            return mrCMSResponse;
        }

        public static void RaiseErrors<T>(this T type)
            where T : AbstractResponseType
        {
            CurrentRequestData.ErrorSignal.Raise(
                new PayPalException(string.Join(", ",
                                                type.Errors.Select(
                                                    errorType =>
                                                    errorType.ErrorCode + " " +
                                                    errorType.LongMessage))));
        }
    }
    public abstract class PayPalResponse
    {
        public bool Success { get { return !Errors.Any(); } }
        public List<string> Errors { get; set; }
    }

    public class SetExpressCheckoutResponse : PayPalResponse
    {
        public string Url { get; set; }
    }

    public class PayPalExpressService : IPayPalExpressService
    {
        private readonly IPayPalRequestService _payPalRequestService;
        private readonly IPayPalUrlService _payPalUrlService;
        private readonly IPayPalInterfaceService _payPalInterfaceService;

        public PayPalExpressService(IPayPalRequestService payPalRequestService, IPayPalUrlService payPalUrlService, IPayPalInterfaceService payPalInterfaceService)
        {
            _payPalRequestService = payPalRequestService;
            _payPalUrlService = payPalUrlService;
            _payPalInterfaceService = payPalInterfaceService;
        }

        public SetExpressCheckoutResponse GetSetExpressCheckoutRedirectUrl(CartModel cart)
        {
            var request = _payPalRequestService.GetSetExpressCheckoutRequest(cart);

            SetExpressCheckoutResponseType setExpressCheckoutResponseType = _payPalInterfaceService.SetExpressCheckout(request);

            return setExpressCheckoutResponseType
                .HandleResponse<SetExpressCheckoutResponseType, SetExpressCheckoutResponse>(
                    (type, response) => { response.Url = _payPalUrlService.GetExpressCheckoutRedirectUrl(type.Token); },
                    (type, response) =>
                        {
                            response.Errors.Add("An error occurred");
                            type.RaiseErrors();
                        });
        }
    }

    public interface IPayPalUrlService
    {
        string GetReturnURL();
        string GetCancelURL();
        string GetExpressCheckoutRedirectUrl(string token);
    }
    public class PayPalUrlService : IPayPalUrlService
    {
        private readonly IDocumentService _documentService;
        private readonly CurrentSite _currentSite;
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;
        private readonly SiteSettings _siteSettings;

        public PayPalUrlService(IDocumentService documentService, CurrentSite currentSite, PayPalExpressCheckoutSettings payPalExpressCheckoutSettings, SiteSettings siteSettings)
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