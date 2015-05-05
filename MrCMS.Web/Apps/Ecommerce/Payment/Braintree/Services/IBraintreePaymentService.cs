using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Payment.Braintree.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Braintree.Services
{
    public interface IBraintreePaymentService
    {
        string GenerateClientToken();
        BraintreeResponse MakePayment(FormCollection collection);
        IEnumerable<SelectListItem> ExpiryMonths();
        IEnumerable<SelectListItem> ExpiryYears();
    }
}