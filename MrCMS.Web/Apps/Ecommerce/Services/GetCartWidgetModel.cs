using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Widgets;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetCartWidgetModel : GetWidgetModelBase<CartWidget>
    {
        private readonly CartModel _cart;

        public GetCartWidgetModel(CartModel cart)
        {
            _cart = cart;
        }

        public override object GetModel(CartWidget widget)
        {
            return _cart;
        }
    }
}