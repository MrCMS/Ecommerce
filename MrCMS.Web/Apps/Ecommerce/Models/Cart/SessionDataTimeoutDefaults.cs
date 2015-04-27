using System;

namespace MrCMS.Web.Apps.Ecommerce.Models.Cart
{
    public static class SessionDataTimeoutDefaults
    {
        public static readonly TimeSpan PaymentInfo = TimeSpan.FromMinutes(20);
    }
}