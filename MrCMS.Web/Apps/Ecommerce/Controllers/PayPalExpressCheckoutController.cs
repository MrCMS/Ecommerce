using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;
using NHibernate.Mapping;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class PayPalExpressCheckoutController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IPayPalExpressService _payPalExpressService;
        private readonly CartModel _cart;
        private readonly IOrderPlacementService _orderPlacementService;
        private readonly IPayPalIPNService _payPalIPNService;
        private readonly IUniquePageService _uniquePageService;

        public PayPalExpressCheckoutController(IPayPalExpressService payPalExpressService, CartModel cart,
            IOrderPlacementService orderPlacementService, IPayPalIPNService payPalIPNService, IUniquePageService uniquePageService)
        {
            _payPalExpressService = payPalExpressService;
            _cart = cart;
            _orderPlacementService = orderPlacementService;
            _payPalIPNService = payPalIPNService;
            _uniquePageService = uniquePageService;
        }

        [HttpGet]
        public PartialViewResult Form()
        {
            return PartialView();
        }

        [HttpPost]
        [ActionName("Form")]
        [ForceImmediateLuceneUpdate]
        public RedirectResult Form_POST()
        {
            if (!_cart.CanPlaceOrder)
            {
                _cart.CannotPlaceOrderReasons.ForEach(s => TempData.ErrorMessages().Add(s));
                return _uniquePageService.RedirectTo<PaymentDetails>();
            }

            var response = _payPalExpressService.DoExpressCheckout(_cart);

            if (response.Success)
            {
                var order = _orderPlacementService.PlaceOrder(_cart, response.UpdateOrder);
                return Redirect(UniquePageHelper.GetUrl<OrderPlaced>(new { id = order.Guid }));
            }

            else
                TempData["error-details"] = new FailureDetails
                {
                    Message =
                        "An error occurred processing your PayPal Express order, please contact the merchant"
                };
            return _uniquePageService.RedirectTo<PaymentDetails>();
        }

        [HttpPost]
        public ActionResult SetExpressCheckout()
        {
            var response = _payPalExpressService.GetSetExpressCheckoutRedirectUrl(_cart);

            return response.Success
                       ? Redirect(response.Url)
                       : _uniquePageService.RedirectTo<Cart>();
        }

        public ActionResult Return(string token)
        {
            var response = _payPalExpressService.ProcessReturn(token);

            return response.Success
                       ? _uniquePageService.RedirectTo<SetShippingDetails>()
                       : _uniquePageService.RedirectTo<Cart>();
        }

        public ActionResult IPN()
        {
            byte[] param = Request.BinaryRead(Request.ContentLength);
            string ipnData = Encoding.ASCII.GetString(param);

            _payPalIPNService.HandleIPN(ipnData);

            return Content("");
        }

        
    }

    public class PayPalExpressCallbackController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGetPaypalShippingOptions _getPaypalShippingOptions;

        public PayPalExpressCallbackController(IGetPaypalShippingOptions getPaypalShippingOptions)
        {
            _getPaypalShippingOptions = getPaypalShippingOptions;
        }

        public ActionResult Handler(PaypalShippingInfo info)
        {
            var options = _getPaypalShippingOptions.Get(info);
            var encoder = new NVPCodec();

            encoder["METHOD"] = "CallbackResponse";
            encoder["CURRENCYCODE"] = "GBP";

            for (int index = 0; index < options.Count; index++)
            {
                var option = options[index];
                encoder[string.Format("L_SHIPPINGOPTIONNAME{0}", index)] = option.SystemName;
                encoder[string.Format("L_SHIPPINGOPTIONLABEL{0}", index)] = option.DisplayName;
                encoder[string.Format("L_SHIPPINGOPTIONAMOUNT{0}", index)] = option.Amount.ToString("0.00");
                //encoder[string.Format("L_TAXAMT{0}", index)] = option.Tax.ToString("0.00");
                encoder[string.Format("L_SHIPPINGOPTIONISDEFAULT{0}", index)] = (option.Default ? "true" : "false");
            }

            Response.Write(encoder.Encode());
            return Content("");
        }
    }

    public class CallbackResponseResult : ActionResult
    {
        private readonly List<PaypalShippingOption> _options;

        public CallbackResponseResult(List<PaypalShippingOption> options)
        {
            _options = options;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            
        }
    }

    public class PaypalShippingOption
    {
        public string DisplayName { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal InsuranceAmount { get; set; }
        public bool Default { get; set; }
        public string SystemName { get; set; }
    }


    public class PaypalShippingInfo
    {
        public string ShipToStreet { get; set; }
        public string ShipToCity {get; set; }
        public string ShipToState { get; set; }
        public string ShipToCountry { get; set; }
        public string ShipToZip { get; set; }
        public string ShipToStreet2 { get; set; }
        public string Token { get; set; }

        public Address ToAddress()
        {
            return new Address
            {
                Address1 = ShipToStreet,
                Address2 = ShipToStreet2,
                City = ShipToCity,
                CountryCode = ShipToCountry,
                StateProvince = ShipToState,
                PostalCode = ShipToZip
            };
        }
    }

    public sealed class NVPCodec : NameValueCollection
    {
        private const string AMPERSAND = "&";
        private const string EQUALS = "=";
        private static readonly char[] AMPERSAND_CHAR_ARRAY = AMPERSAND.ToCharArray();
        private static readonly char[] EQUALS_CHAR_ARRAY = EQUALS.ToCharArray();

        public string Encode()
        {
            StringBuilder sb = new StringBuilder();
            bool firstPair = true;
            foreach (string kv in AllKeys)
            {
                string name = HttpUtility.UrlEncode(kv);
                string value = HttpUtility.UrlEncode(this[kv]);
                if (!firstPair)
                {
                    sb.Append(AMPERSAND);
                }
                sb.Append(name).Append(EQUALS).Append(value);
                firstPair = false;
            }
            return sb.ToString();
        }

        public void Decode(string nvpstring)
        {
            Clear();
            foreach (string nvp in nvpstring.Split(AMPERSAND_CHAR_ARRAY))
            {
                string[] tokens = nvp.Split(EQUALS_CHAR_ARRAY);
                if (tokens.Length >= 2)
                {
                    string name = HttpUtility.UrlDecode(tokens[0]);
                    string value = HttpUtility.UrlDecode(tokens[1]);
                    Add(name, value);
                }
            }
        }

        public void Add(string name, string value, int index)
        {
            this.Add(GetArrayName(index, name), value);
        }

        public void Remove(string arrayName, int index)
        {
            this.Remove(GetArrayName(index, arrayName));
        }

        public string this[string name, int index]
        {
            get
            {
                return this[GetArrayName(index, name)];
            }
            set
            {
                this[GetArrayName(index, name)] = value;
            }
        }

        private static string GetArrayName(int index, string name)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "index cannot be negative : " + index);
            }
            return name + index;
        }
    }
}