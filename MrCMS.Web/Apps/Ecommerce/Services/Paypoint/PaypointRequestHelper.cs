using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Paypoint;

namespace MrCMS.Web.Apps.Ecommerce.Services.Paypoint
{
    public class PaypointRequestHelper : IPaypointRequestHelper
    {
        private readonly PaypointSettings _paypointSettings;
        private readonly IPaypointResponseLogger _paypointResponseLogger;

        public PaypointRequestHelper(PaypointSettings paypointSettings, IPaypointResponseLogger paypointResponseLogger)
        {
            _paypointSettings = paypointSettings;
            _paypointResponseLogger = paypointResponseLogger;
        }

        public string GetTotal(decimal total)
        {
            return total.ToString("0.00");
        }

        public string GetOptions(PaypointPaymentDetailsModel model)
        {
            var options = string.Format("test_status={0},card_type={1},cv2={2},mail_customer=false",
                                        _paypointSettings.IsLive ? "live" : "true", model.CardType,
                                        model.CardVerificationCode);
            if (!_paypointSettings.IsLive)
                options += ",dups=false";
            return options;
        }

        public string GetDate(int? month, int? year)
        {
            if (month.GetValueOrDefault() == 0 || year.GetValueOrDefault() == 0)
                return string.Empty;

            var yearString = year.ToString();
            return string.Format("{0}{1}", month == null ? "00" : month.ToString().PadLeft(2, '0'),
                                 year == null ? "00" : yearString.PadLeft(2, '0'));
        }

        public NameValueCollection ParseResponse(string response)
        {
            response = response.Trim('?');
            var parameterList = response.Split('&');
            var enrolmentResponse = parameterList.ToDictionary(s => s.Split('=')[0],
                                                               s => s.Substring(s.Split('=')[0].Length + 1));

            var nameValueCollection = new NameValueCollection();
            foreach (var key in enrolmentResponse.Keys)
                nameValueCollection.Add(key, enrolmentResponse[key]);

            if (_paypointSettings.LogResponses)
            {
                _paypointResponseLogger.LogResponse(response, nameValueCollection);
            }

            return nameValueCollection;
        }

        public string GetAddress(Address address, string email)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("name={0},", address.Name);
            stringBuilder.AppendFormat("company={0},", address.Company);
            stringBuilder.AppendFormat("addr_1={0},", address.Address1);
            stringBuilder.AppendFormat("addr_2={0},", address.Address2);
            stringBuilder.AppendFormat("city={0},", address.City);
            stringBuilder.AppendFormat("state={0},", address.StateProvince);
            stringBuilder.AppendFormat("post_code={0},", address.PostalCode);
            stringBuilder.AppendFormat("country={0},", address.Country.Name);
            stringBuilder.AppendFormat("email={0},", email);
            return stringBuilder.ToString();
        }

        public string GetOrderDetails(CartModel cartModel)
        {
            var lineData =
                cartModel.Items.Select(
                    line =>
                    string.Format("prod={0},item_amount={1:0.00}x{2}", line.Name, line.UnitPrice, line.Quantity))
                         .ToList();
            if (cartModel.OrderTotalDiscount > 0)
            {
                lineData.Add(string.Format("prod=DISCOUNT,item_amount={0:N2}", cartModel.OrderTotalDiscount));
            }
            return string.Join(";", lineData);
        }
    }
}