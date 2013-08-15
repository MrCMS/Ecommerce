namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class ProcessDetailsResponse
    {
        public bool PaymentFailed { get { return FailureDetails != null; } }
        public bool PaymentSucceeded { get { return PaypointPaymentDetails != null; } }
        public bool Requires3DSecure { get { return RedirectDetails != null; } }

        public FailureDetails FailureDetails { get; set; }
        public RedirectDetails RedirectDetails { get; set; }
        public PaypointPaymentDetails PaypointPaymentDetails { get; set; }

        public bool ErrorOccurred { get; set; }
    }
}