using System;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface ICartGuidResetter
    {
        Guid ResetCartGuid(Guid userGuid);
    }
}