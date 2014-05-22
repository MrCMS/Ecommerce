namespace MrCMS.Web.Apps.Ecommerce.Payment.WorldPay.Models
{
    public class WorldPayPostInfo
    {
        public WorldPayPostInfo()
        {
            hideContact = "true";
            noLanguageMenu = "true";
            fixContact = "false";
        }
        public string PostUrl { get; set; }
        public bool HasPaymentType { get { return !string.IsNullOrWhiteSpace(paymentType); } }
        public bool HasCSSScheme { get { return !string.IsNullOrWhiteSpace(MC_WorldPayCSSName); } }
        public bool HideShipping { get; set; }

        // ReSharper disable InconsistentNaming
        public string instId { get; set; }
        public string cartId { get; set; }
        public string paymentType { get; set; }
        public string MC_WorldPayCSSName { get; set; }
        public string currency { get; set; }
        public string email { get; set; }
        public string hideContact { get; set; }
        public string noLanguageMenu { get; set; }
        public string withDelivery { get; set; }
        public string fixContact { get; set; }
        public string amount { get; set; }
        public string desc { get; set; }

        public string M_UserID { get; set; }
        public string name { get; set; }
        public string M_FirstName { get; set; }
        public string M_LastName { get; set; }
        public string M_Addr1 { get; set; }
        public string address { get; set; }
        public string tel { get; set; }
        public string M_Addr2 { get; set; }
        public string M_Business { get; set; }
        public string lang { get; set; }
        public string M_StateCounty { get; set; }
        public string postcode { get; set; }
        public string country { get; set; }

        public string testMode { get; set; }
        public string MC_callback { get; set; }

        public string delvName { get; set; }
        public string delvAddress { get; set; }
        public string delvPostcode { get; set; }
        public string delvCountry { get; set; }

        // ReSharper restore InconsistentNaming
    }
}