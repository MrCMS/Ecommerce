namespace MrCMS.Web.Apps.Ecommerce.Payment.Elavon.Models
{
    public class ElavonCustomEnumerations
    {
        public enum ResultType
        {
            ChargeSuccess,
            ChargeFailure,
            ChargeNotFound,
            TamperedHashException,
            TamperedTotalPay,
            CommsException,
            BadRequest
        }

        //For exhaustive list of status refer Elavon dashboard 
        public enum PaymentTransactionStatus
        {
            Successful,
            Declined,
            Authorised,
            Uncaptured
        }
    }
}