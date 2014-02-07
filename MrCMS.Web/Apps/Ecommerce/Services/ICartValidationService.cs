using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface ICartValidationService
    {
        CanAddQuantityValidationResult CanAddQuantity(AddToCartModel model);
    }
}