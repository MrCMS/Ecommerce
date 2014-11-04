using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Widgets;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetSpendXMoreData : GetWidgetModelBase<SpendXMore>
    {
        private readonly CartModel _cart;

        public GetSpendXMoreData(CartModel cart)
        {
            _cart = cart;
        }

        public override object GetModel(SpendXMore widget)
        {
            var model = new SpendXMoreModel { SpendAmountMore = 0 };

            if (_cart == null) return model;

            var amount = widget.Amount;
            if (_cart.Subtotal > 0 && _cart.Subtotal < amount)
                model.SpendAmountMore = amount - (_cart.Subtotal + _cart.ItemTax);

            return model;
        }
    }
}