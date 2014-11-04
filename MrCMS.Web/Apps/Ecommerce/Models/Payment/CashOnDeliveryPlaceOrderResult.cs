using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Models.Payment
{
    public class CashOnDeliveryPlaceOrderResult
    {
        public CashOnDeliveryPlaceOrderResult()
        {
            CannotPlaceOrderReasons = new List<string>();
        }
        public List<string> CannotPlaceOrderReasons { get; set; }
        public RedirectResult RedirectResult { get; set; }
    }
}