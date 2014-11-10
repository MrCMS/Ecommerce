using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class AddToCartController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IAddToCartUIService _addToCartUIService;

        public AddToCartController(IAddToCartUIService addToCartUIService)
        {
            _addToCartUIService = addToCartUIService;
        }

        public PartialViewResult Add(AddToCartModel model)
        {
            return PartialView(model);
        }

        [HttpPost]
        [ActionName("Add")]
        public RedirectResult Add_POST([IoCModelBinder(typeof(AddToCartModelBinder))]AddToCartModel model)
        {
            var addToCartResult = _addToCartUIService.Add(model);
            return _addToCartUIService.Redirect(addToCartResult);
        }
    }
}