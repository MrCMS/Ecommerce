using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Filters
{

    public class CanEnterCheckout : ActionFilterAttribute
    {
        private CartModel _getCartModel;
        private IUniquePageService _getUniquePageService;

        public CartModel GetCartModel
        {
            get { return _getCartModel ?? MrCMSApplication.Get<CartModel>(); }
            set { _getCartModel = value; }
        }

        public IUniquePageService GetUniquePageService
        {
            get { return _getUniquePageService ?? MrCMSApplication.Get<IUniquePageService>(); }
            set { _getUniquePageService = value; }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!GetCartModel.CanCheckout)
            {
                filterContext.Result = GetUniquePageService.RedirectTo<Cart>();
            }
        }
    }
}