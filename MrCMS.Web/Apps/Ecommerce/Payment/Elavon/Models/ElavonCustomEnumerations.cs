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


        public enum ElavonChargeExceptionType
        {
            None,
            TamperedTotalPay,
            ChargeFailure,
            Other
        }
    }
}