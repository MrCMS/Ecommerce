using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.CharityClear.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using Newtonsoft.Json;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Payment.CharityClear.Services
{
    public class CharityClearPaymentService : ICharityClearPaymentService
    {
        private readonly CartModel _cart;
        private readonly ICartBuilder _cartBuilder;
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly IOrderPlacementService _orderPlacementService;
        private readonly ISession _session;
        private readonly Site _site;
        private readonly CharityClearSettings _charityClearSettings;

        public CharityClearPaymentService(CartModel cart,
            EcommerceSettings ecommerceSettings, ICartBuilder cartBuilder,
            ISession session, IOrderPlacementService orderPlacementService, Site site, CharityClearSettings charityClearSettings)
        {
            _cart = cart;
            _ecommerceSettings = ecommerceSettings;
            _cartBuilder = cartBuilder;
            _session = session;
            _orderPlacementService = orderPlacementService;
            _site = site;
            _charityClearSettings = charityClearSettings;
        }

        public CharityClearPostModel GetInfo()
        {
            var charityClearPostInfo = new CharityClearPostModel();

            var fields = GetBaseFields();

            string calculateHash = CalculateHash(fields, _charityClearSettings.SignatureKey);
            fields.Add("signature", calculateHash);
            charityClearPostInfo.Fields = fields;
            return charityClearPostInfo;
        }

        public SortedDictionary<string, string> GetBaseFields()
        {
            string returnUrl = string.Format("{0}/Apps/Ecommerce/CharityClear/Notification", GetSiteUrl());
            string total = _cart.TotalToPay.ToString("0.00").Replace(".", "");
            var fields = new SortedDictionary<string, string>
            {
                {"merchantID", _charityClearSettings.MerchantId},
                {"amount", total.Replace(".","")},
                {"action", "SALE"},
                {"type", "1"},
                {"countryCode", _charityClearSettings.ISOCountryCode},
                {"currencyCode", _ecommerceSettings.CurrencyCode()},
                {"orderRef", _cart.CartGuid.ToString()},
                {"redirectURL", returnUrl}
            };
            if (!string.IsNullOrWhiteSpace(_charityClearSettings.MerchantPassword))
            {
                fields.Add("merchantPwd", _charityClearSettings.MerchantPassword);
            }
            if (_cart.BillingAddress != null)
            {
                fields.Add("customerName", _cart.BillingAddress.Name.Trim());
                string address =
                    _cart.BillingAddress.GetDescription(true)
                        .Replace(_cart.BillingAddress.PostalCode, "")
                        .Replace(",", "\n");
                fields.Add("customerAddress", address);
                fields.Add("customerPostCode", _cart.BillingAddress.PostalCode);
                fields.Add("customerPhone", _cart.BillingAddress.PhoneNumber);
                fields.Add("customerEmail", _cart.OrderEmail);
            }

            int i = 1;
            foreach (var cartItem in _cart.Items)
            {
                fields.Add("item" + i + "Description", cartItem.Name);
                fields.Add("item" + i + "Quantity", cartItem.Quantity.ToString());
                fields.Add("item" + i + "GrossValue", cartItem.Price.ToString("0.00").Replace(".", ""));
                i++;
            }

            fields.Add("merchantData", CalculateHash(_cart.CartGuid + _charityClearSettings.SignatureKey));

            return fields;
        }

        private string CalculateHash(SortedDictionary<string, string> fields, string signatureKey)
        {
            var stringToHash = ToRequest(fields) + signatureKey;
            return CalculateHash(stringToHash);
        }

        private string CalculateHash(string toHash)
        {
            using (SHA512 shaM = new SHA512Managed())
            {
                return BitConverter.ToString(shaM.ComputeHash(Encoding.UTF8.GetBytes(toHash))).Replace("-", "").ToLower();
            }
        }

        private string ToRequest(SortedDictionary<string, string> fields)
        {
            var values = new List<string>();
            foreach (var item in fields.Where(x=>x.Key != "signature"))
            {
                values.Add(string.Format("{0}={1}", item.Key,
                    UrlEncodeUpperCase(item.Value)));
            }
            return string.Join("&", values.ToArray());
        }

        //this is needed as .net url encodes with ToLower and PHP uses upper case. Therefore the hash never matched.
        static string UrlEncodeUpperCase(string value)
        {
            value = HttpUtility.UrlEncode(value);
            return Regex.Replace(value, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());
        }

        private string GetSiteUrl()
        {
            string authority = _site.BaseUrl;
            if (authority.EndsWith("/"))
                authority = authority.TrimEnd('/');
            return string.Format("{0}{1}", "http://", authority);
        }

        public SortedDictionary<string, string> ToDictionary(NameValueCollection col)
        {
            var dict = new SortedDictionary<string, string>();
            foreach (var key in col.Keys)
            {
                dict.Add(key.ToString(), col[key.ToString()]);
            }
            return dict;
        }

        public CharityClearResponse HandleNotification(FormCollection form)
        {
            if (form["responseCode"] == "0")
            {
                
                string captureTransactionId = form["transactionID"];
                var amountCharged = form["amountReceived"];
                string orderId = form["orderRef"];
                string hash = form["merchantData"];
                var cart = GetCart(orderId);

                try
                {
                    if (!CheckHash(hash, orderId))
                    {
                        throw new Exception(string.Format("We could not validate this payment. Please contact us quoting reference {0}", captureTransactionId));
                    }
                    if (cart == null)
                    {
                        throw new Exception(string.Format("The order ID {0} doesn't exist", orderId));
                    }
                }
                catch (Exception exception)
                {
                    CurrentRequestData.ErrorSignal.Raise(exception);
                    return new CharityClearResponse
                    {
                        ErrorMessages =
                            new List<string>
                            {
                                string.Format("Sorry, something went wrong. Please contact us quoting reference {0}", captureTransactionId)
                            }
                    };
                }

                if (amountCharged != cart.TotalToPay.ToString("0.00").Replace(".", ""))
                {
                    return new CharityClearResponse
                    {
                        ErrorMessages =
                            new List<string>
                            {
                                string.Format("Something went wrong with the amount we charged you. Please contact us quoting transaction {0}", captureTransactionId)
                            }
                    };
                }

                Order order = _orderPlacementService.PlaceOrder(cart, o =>
                {
                    o.PaymentStatus = PaymentStatus.Paid;
                    o.ShippingStatus = ShippingStatus.Unshipped;
                    o.CaptureTransactionId = captureTransactionId;
                });

                return new CharityClearResponse
                {
                    Order = order,
                    Success = true
                };
            }

            return new CharityClearResponse
            {
                ErrorMessages =
                    new List<string>
                            {
                                "Your transaction was not authorised. Please check your details and try again.", form["responseMessage"]
                            }
            };
        }

        private bool CheckHash(string hash, string orderId)
        {
            var calculatedHash = CalculateHash(orderId + _charityClearSettings.SignatureKey);
            return calculatedHash == hash;
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

    }

    public class CharityClearResponse
    {
        public bool Success { get; set; }

        public Order Order { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}