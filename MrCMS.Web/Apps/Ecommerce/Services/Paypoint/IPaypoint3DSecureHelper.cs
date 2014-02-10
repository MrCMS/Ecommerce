using System;

namespace MrCMS.Web.Apps.Ecommerce.Services.Paypoint
{
    public interface IPaypoint3DSecureHelper
    {
        Guid ResetCartGuid();
        decimal GetOrderAmount();
        void SetOrderAmount(decimal total);
        Guid GetCartGuid();
        void SetCartGuid(Guid cartGuid);
    }
}