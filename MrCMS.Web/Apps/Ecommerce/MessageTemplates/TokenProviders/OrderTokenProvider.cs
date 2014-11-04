using System;
using System.Collections.Generic;
using System.Text;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates.TokenProviders
{
    public class OrderTokenProvider : ITokenProvider<Order>
    {
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
                               "ShippingAddressFormattedHtml",
                               order =>
                               order.ShippingAddress != null
                                   ? order.ShippingAddress.GetDescription().Replace(",", ",<br />")
                                   : string.Empty
                           },
                           {
                               "ShoppingCartHtml", order =>
                                                       {
                                                           var sb = new StringBuilder();
                                                           sb.Append(
                                                               "<table cellpadding=2 cellspacing=2 border=0 style='border: 1px solid grey;'>");
                                                           sb.Append("<tr>");
                                                           sb.Append("<td>Product</td>");
                                                           sb.Append("<td>Quantity</td>");
                                                           sb.Append("<td>Item Price</td>");
                                                           sb.Append("<td>Total</td>");
                                                           sb.Append("</tr>");

                                                           foreach (var item in order.OrderLines)
                                                           {
                                                               sb.Append("<tr>");
                                                               sb.Append("<td>" + item.Name + " (" + item.SKU + ")" +
                                                                         GetDownloadLink(item) + "</td>");
                                                               sb.Append("<td>" + item.Quantity + "</td>");
                                                               sb.Append("<td>" + item.UnitPrice.ToCurrencyFormat() + "</td>");
                                                               sb.Append("<td>" + item.Subtotal.ToCurrencyFormat() + "</td>");
                                                               sb.Append("</tr>");
                                                           }

                                                           sb.Append("</table>");
                                                           sb.Append("<br />");
                                                           sb.Append("<p>");
                                                           sb.AppendFormat("Subtotal: {0}<br />", order.Subtotal.ToCurrencyFormat());
                                                           sb.AppendFormat("VAT: {0}<br />", order.Tax.ToCurrencyFormat());
                                                           if (order.ShippingTotal > 0)
                                                           {
                                                               sb.AppendFormat("Shipping: {0} <br />",
                                                                   order.ShippingTotal.ToCurrencyFormat());
                                                           }
                                                           if (order.DiscountAmount > 0)
                                                           {
                                                               sb.AppendFormat("Discount: {0} (Code: {1})<br />", order.DiscountAmount.ToCurrencyFormat(), order.DiscountAmount);
                                                           }
                                                           sb.AppendFormat("<strong>Total: {0}</strong>", order.Total.ToCurrencyFormat());
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