using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface IGetGiftCardDataFromContext
    {
        string Get(ControllerContext controllerContext);
    }
}