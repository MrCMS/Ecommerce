using System;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IPaymentNotRequiredUIService
    {
        ActionResult RedirectToPaymentDetails();
        PaymentNotRequiredResult TryPlaceOrder();
    }

    public class PaymentNotRequiredResult
    {
        public bool Success { get; set; }
        public FailureDetails FailureDetails { get; set; }
        public RedirectResult RedirectTo { get; set; }
    }
}