using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class CartValidationService : ICartValidationService
    {
        private readonly ISession _session;

        public CartValidationService(ISession session)
        {
            _session = session;
        }

        public CanAddQuantityValidationResult CanAddQuantity(AddToCartModel model)
        {
            var variant = model.ProductVariant;

            if (variant == null)
                return new CanAddQuantityValidationResult("Cannot find the selected variant");
            if (model.Quantity <= 0)
                return new CanAddQuantityValidationResult("Cannot add an amount less than 1");
            var canBuy = variant.CanBuy(model.Quantity);
            if (!canBuy.OK)
                return new CanAddQuantityValidationResult(canBuy.Message);
            return CanAddQuantityValidationResult.Successful;
        }
    }
}