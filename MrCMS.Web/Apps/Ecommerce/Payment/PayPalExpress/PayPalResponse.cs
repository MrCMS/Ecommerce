using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public abstract class PayPalResponse
    {
        protected PayPalResponse()
        {
            Errors = new List<string>();
        }
        public bool Success { get { return !Errors.Any(); } }
        public List<string> Errors { get; set; }
    }
}