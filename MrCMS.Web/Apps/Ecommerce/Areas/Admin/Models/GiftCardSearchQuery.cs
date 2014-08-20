namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class GiftCardSearchQuery
    {
        public GiftCardSearchQuery()
        {
            Page = 1;
        }

        public string GiftCode { get; set; }
        public string Recipient { get; set; }
        public string Sender { get; set; }

        public int Page { get; set; }
    }
}