using System.Text;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Analytics
{
    public class GoogleAnalyticsService : IGoogleAnalyticsService
    {
        const string AddTrans = @"_gaq.push(['_addTrans', '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}']);"; 
        const string AddItem = @"_gaq.push(['_addItem', '{0}', '{1}', '{2}', '{3}', '{4}', '{5}']);";
        const string TrackTrans = "_gaq.push(['_trackTrans']);";

        public string GetAnalayticsCode(Order order, string trackingScript)
        {
            if (order == null)
                return trackingScript;

            var trackingScriptContainsEcommerce = trackingScript.Contains("{ecommerce}");
            if (!trackingScriptContainsEcommerce)
                return trackingScript;

            var trackingCodeSb = new StringBuilder();
            var shippingCity = order.ShippingAddress != null ? order.ShippingAddress.City : "";
            var shippingCounty = order.ShippingAddress != null ? order.ShippingAddress.StateProvince : "";
            var shippingCountry = order.ShippingAddress != null ? order.ShippingAddress.Country.Name : "";

            trackingCodeSb.Append(string.Format(AddTrans, order.Id, order.Site.Name, order.Total, order.Tax,
                                                order.ShippingTotal, shippingCity, shippingCounty, shippingCountry));

            foreach (var orderLine in order.OrderLines)
            {
                trackingCodeSb.Append(string.Format(AddItem, order.Id, orderLine.SKU, orderLine.Name, orderLine.Options , orderLine.UnitPrice, orderLine.Quantity));
            }

            trackingCodeSb.Append(TrackTrans);

            return trackingScript.Replace("{ecommerce}", trackingCodeSb.ToString());
        }
    }
}