namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class PaymentDetailsModel
    {
        public CartModel CartModel { get; set; }
        public string CartType { get; set; }
        public string CardNumber { get; set; }
        public string CardVerificationCode { get; set; }
        public int StartMonth { get; set; }
        public int StartYear { get; set; }
        public int EndMonth { get; set; }
        public int EndYear { get; set; }
        public string NameOnCard { get; set; }
        public string CardIssueNumber { get; set; }
        public bool SignUpForNewsletter { get; set; }
    }
}