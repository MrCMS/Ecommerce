using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.WorldPay.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using Newtonsoft.Json;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Payment.WorldPay.Services
{
    public class WorldPayPaymentService : IWorldPayPaymentService
    {
        private readonly CartModel _cart;
        private readonly ICartBuilder _cartBuilder;
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly IOrderPlacementService _orderPlacementService;
        private readonly ISession _session;
        private readonly Site _site;
        private readonly WorldPaySettings _worldPaySettings;

        public WorldPayPaymentService(WorldPaySettings worldPaySettings, CartModel cart,
            EcommerceSettings ecommerceSettings, ICartBuilder cartBuilder,
            ISession session, IOrderPlacementService orderPlacementService, Site site)
        {
            _worldPaySettings = worldPaySettings;
            _cart = cart;
            _ecommerceSettings = ecommerceSettings;
            _cartBuilder = cartBuilder;
            _session = session;
            _orderPlacementService = orderPlacementService;
            _site = site;
        }

        public WorldPayPostInfo GetInfo()
        {
            string schemeAndAuthority = GetSchemeAndAuthority();
            string returnUrl = string.Format("{0}/Apps/Ecommerce/WorldPay/Notification", schemeAndAuthority);

            var postInfo = new WorldPayPostInfo();
            postInfo.PostUrl = _worldPaySettings.GetPostUrl();

            postInfo.instId = _worldPaySettings.InstanceId;
            postInfo.cartId = _cart.CartGuid.ToString();

            if (!string.IsNullOrEmpty(_worldPaySettings.PaymentMethod))
            {
                postInfo.paymentType = _worldPaySettings.PaymentMethod;
            }

            if (!string.IsNullOrEmpty(_worldPaySettings.CssName))
            {
                postInfo.MC_WorldPayCSSName = _worldPaySettings.CssName;
            }

            postInfo.currency = _ecommerceSettings.CurrencyCode();
            postInfo.email = _cart.OrderEmail;
            postInfo.withDelivery = _cart.RequiresShipping ? "true" : "false";
            postInfo.amount = _cart.TotalToPay.ToString(new CultureInfo("en-US", false).NumberFormat);
            postInfo.desc = _site.Name;
            postInfo.M_UserID = _cart.UserGuid.ToString();
            postInfo.M_FirstName = _cart.BillingAddress.FirstName;
            postInfo.M_LastName = _cart.BillingAddress.LastName;
            postInfo.M_Addr1 = _cart.BillingAddress.Address1;
            postInfo.tel = _cart.BillingAddress.PhoneNumber;
            postInfo.M_Addr2 = _cart.BillingAddress.Address2;
            postInfo.M_Business = _cart.BillingAddress.Company;

            postInfo.lang = CurrentRequestData.CultureInfo.TwoLetterISOLanguageName;

            postInfo.M_StateCounty = _cart.BillingAddress.StateProvince;

            postInfo.testMode = _worldPaySettings.UseSandbox ? "100" : "0";
            postInfo.postcode = _cart.BillingAddress.PostalCode;
            postInfo.country = _cart.BillingAddress.CountryCode;

            postInfo.address = string.Format("{0} {1}", _cart.BillingAddress.Address1,
                _cart.BillingAddress.GetCountryName());
            postInfo.MC_callback = returnUrl;
            postInfo.name = string.Format("{0} {1}", _cart.BillingAddress.FirstName, _cart.BillingAddress.LastName);

            if (_cart.RequiresShipping)
            {
                postInfo.delvName = string.Format("{0} {1}", _cart.ShippingAddress.FirstName,
                    _cart.ShippingAddress.LastName);
                string delvAddress = _cart.ShippingAddress.Address1;
                delvAddress += (!string.IsNullOrEmpty(_cart.ShippingAddress.Address2))
                    ? string.Format(" {0}", _cart.ShippingAddress.Address2)
                    : string.Empty;
                postInfo.delvAddress = delvAddress;
                postInfo.delvPostcode = _cart.ShippingAddress.PostalCode;
                postInfo.delvCountry = _cart.ShippingAddress.CountryCode;
            }
            else
            {
                postInfo.HideShipping = true;
            }
            return postInfo;
        }

        public ActionResult HandleNotification(HttpRequestBase request)
        {
            NameValueCollection form = request.Form;
            NameValueCollection queryString = request.QueryString;
            string transStatus = form["transStatus"] ?? string.Empty;
            string returnedcallbackPw = form["callbackPW"] ?? string.Empty;
            string orderId = form["cartId"] ?? string.Empty;
            string returnedInstanceId = form["instId"] ?? string.Empty;
            string callbackPassword = _worldPaySettings.CallbackPassword;
            string transId = form["transId"] ?? string.Empty;
            string transResult = queryString["msg"] ?? string.Empty;
            string authCode = queryString["rawAuthMessage"] ?? string.Empty;
            decimal amount;
            decimal.TryParse(queryString["authAmount"], out amount);
            string instanceId = _worldPaySettings.InstanceId;

            CartModel cart = GetCart(orderId);
            try
            {
                if (cart == null)
                    throw new Exception(string.Format("The order ID {0} doesn't exist", orderId));

                if (string.IsNullOrEmpty(instanceId))
                    throw new Exception("Worldpay Instance ID is not set");

                if (string.IsNullOrEmpty(returnedInstanceId))
                    throw new Exception("Returned Worldpay Instance ID is not set");

                if (instanceId.Trim() != returnedInstanceId.Trim())
                    throw new Exception(
                        string.Format(
                            "The Instance ID ({0}) received for order {1} does not match the WorldPay Instance ID stored in the database ({2})",
                            returnedInstanceId, orderId, instanceId));

                if (returnedcallbackPw.Trim() != callbackPassword.Trim())
                    throw new Exception(
                        string.Format(
                            "The callback password ({0}) received within the Worldpay Callback for the order {1} does not match that stored in your database.",
                            returnedcallbackPw, orderId));

                if (transStatus.ToLower() != "y")
                    throw new Exception(
                        string.Format(
                            "The transaction status received from WorldPay ({0}) for the order {1} was declined.",
                            transStatus, orderId));
                if (cart.TotalToPay != amount)
                {
                    throw new Exception(
                        string.Format("The paid amount {0} does not match the amount to pay {1}", amount,
                            cart.TotalToPay));
                }
            }
            catch (Exception exception)
            {
                CurrentRequestData.ErrorSignal.Raise(exception);
                return new ContentResult
                {
                    Content = exception.Message
                };
            }
            Order order = _orderPlacementService.PlaceOrder(cart, o =>
            {
                o.PaymentStatus = PaymentStatus.Paid;
                o.ShippingStatus = ShippingStatus.Unshipped;
                o.AuthorisationToken = authCode;
                o.CaptureTransactionId = transId;
            });

            return new ContentResult()
            {
                Content = "OK"
            };
        }

        private CartModel GetCart(string orderId)
        {
            string serializedTxCode = JsonConvert.SerializeObject(orderId);
            SessionData sessionData =
                _session.QueryOver<SessionData>()
                    .Where(data => data.Key == CartManager.CurrentCartGuid && data.Data == serializedTxCode)
                    .SingleOrDefault();
            if (sessionData != null)
            {
                CurrentRequestData.UserGuid = sessionData.UserGuid;
                return _cartBuilder.BuildCart(sessionData.UserGuid);
            }
            return null;
        }

        private string GetSchemeAndAuthority()
        {
            string scheme = _worldPaySettings.RequiresSSL ? "https://" : "http://";
            string authority = _site.BaseUrl;
            if (authority.EndsWith("/"))
                authority = authority.TrimEnd('/');
            return string.Format("{0}{1}", scheme, authority);
        }
    }
}