using MrCMS.Web.Apps.Ecommerce.Entities.Users;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress.Models
{
    public class PaypalShippingInfo
    {
        public string ShipToStreet { get; set; }
        public string ShipToCity { get; set; }
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
}