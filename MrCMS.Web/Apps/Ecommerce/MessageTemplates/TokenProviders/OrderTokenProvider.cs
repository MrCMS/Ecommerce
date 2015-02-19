using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates.TokenProviders
{
    public class OrderTokenProvider : ITokenProvider<Order>
    {
        private readonly TaxSettings _taxSettings;
        private readonly StringResourceProvider _stringResourceProvider;

        public OrderTokenProvider(TaxSettings taxSettings, StringResourceProvider stringResourceProvider)
        {
            _taxSettings = taxSettings;
            _stringResourceProvider = stringResourceProvider;
        }

        private IDictionary<string, Func<Order, string>> _tokens;
        public IDictionary<string, Func<Order, string>> Tokens { get { return _tokens = _tokens ?? GetTokens(); } }

        private IDictionary<string, Func<Order, string>> GetTokens()
        {
            return new Dictionary<string, Func<Order, string>>
                       {
                           {
                               "CustomerName",
                               order =>
                               order.User != null
                                   ? order.User.Name
                                   : (order.BillingAddress != null ? order.BillingAddress.Name : "Customer")
                           },
                           {
                               "CustomerFirstName",
                               order =>
                               order.User != null
                                   ? order.User.FirstName
                                   : (order.BillingAddress != null ? order.BillingAddress.FirstName : string.Empty)
                           },
                           {
                               "CustomerLastName",
                               order =>
                               order.User != null
                                   ? order.User.LastName
                                   : (order.BillingAddress != null ? order.BillingAddress.LastName : string.Empty)
                           },
                           {"BillingAddressFormatted", order => order.BillingAddress.GetDescription()},
                           {
                               "ShippingAddressFormatted",
                               order =>
                               order.ShippingAddress != null ? order.ShippingAddress.GetDescription() : string.Empty
                           },
                           {
                               "BillingAddressFormattedHtml",
                               order =>
                               order.BillingAddress != null
                                   ? order.BillingAddress.GetDescription().Replace(",", ",<br />")
                                   : string.Empty
                           },
                           {
                               "BillingAddressPhoneNumber",
                               order =>
                               order.BillingAddress != null
                                   ? order.BillingAddress.PhoneNumber
                                   : string.Empty
                           },
                           {
                               "ShippingAddressFormattedHtml",
                               order =>
                               order.ShippingAddress != null
                                   ? order.ShippingAddress.GetDescription().Replace(",", ",<br />")
                                   : string.Empty
                           },
                           {
                               "ShippingAddressPhoneNumber",
                               order =>
                               order.ShippingAddress != null
                                   ? order.ShippingAddress.PhoneNumber
                                   : string.Empty
                           },
                           {
                               "ShoppingCartHtml", order =>
                                                       {
                                                           var sb = new StringBuilder();
                                                            sb.Append(
                                                            "<table cellpadding=2 cellspacing=2 style='border: 1px solid grey;' border='1'>");
                                                            sb.Append("<tr>");
                                                            sb.Append("<td>" + _stringResourceProvider.GetValue("Product") +"</td>");
                                                            sb.Append("<td>" + _stringResourceProvider.GetValue("Quantity") + "</td>");
                                                            sb.Append("<td>" + _stringResourceProvider.GetValue("Price (ex TAX)") + "</td>");
                                                            if (_taxSettings.TaxesEnabled)
                                                                sb.Append("<td>" + _stringResourceProvider.GetValue("Tax Rate") + "</td>");
                                                           
                                                            sb.Append("<td>" + _stringResourceProvider.GetValue("Net Sub Total") + "</td>");
                                                            sb.Append("</tr>");

                                                            foreach (var item in order.OrderLines)
                                                            {
                                                                sb.Append("<tr>");
                                                                sb.Append("<td>" + item.Name + " (" + item.SKU + ")" +
                                                                            GetDownloadLink(item) + "</td>");
                                                                sb.Append("<td>" + item.Quantity + "</td>");
                                                                sb.Append("<td>" + item.UnitPricePreTax.ToCurrencyFormat() + "</td>");
                                                                if(_taxSettings.TaxesEnabled)
                                                                    sb.Append("<td>" + item.TaxRate.ToString("0") + "%</td>");
                                                                sb.Append("<td>" + (item.Subtotal - item.Tax).ToCurrencyFormat() + "</td>");
                                                                sb.Append("</tr>");
                                                            }

                                                            sb.Append("</table>");
                                                            sb.Append("<br />");
                                                            sb.Append("<p>");
                                                            sb.AppendFormat(_stringResourceProvider.GetValue("Net Sub Total") + ": {0}<br />", order.Subtotal.ToCurrencyFormat());
                                                            sb.AppendFormat(_stringResourceProvider.GetValue("Tax") + ": {0}<br />", order.Tax.ToCurrencyFormat());
                                                            if (order.ShippingTotal > 0)
                                                            {
                                                                sb.AppendFormat(_stringResourceProvider.GetValue("Shipping") + ": {0} <br />",
                                                                    order.ShippingTotal.ToCurrencyFormat());
                                                            }
                                                            if (order.DiscountAmount > 0)
                                                            {
                                                                sb.AppendFormat(_stringResourceProvider.GetValue("Discount") + ": {0}<br />", order.DiscountAmount.ToCurrencyFormat());
                                                            }
                                                            sb.AppendFormat("<strong>"+ _stringResourceProvider.GetValue("Total")+": {0}</strong>", order.Total.ToCurrencyFormat());
                                                            sb.Append("</p>");
                                                           
                                                           return sb.ToString();
                                                       }
                           }
                       };
        }

        private string GetDownloadLink(OrderLine item)
        {
            if (!item.IsDownloadable)
                return string.Empty;
            return string.Format(" - <a href=\"{0}\">Download</a>", item.DownloadMaskedLink);
        }
    }
}