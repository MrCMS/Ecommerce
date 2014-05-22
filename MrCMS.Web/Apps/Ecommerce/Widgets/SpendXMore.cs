using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class SpendXMore : Widget
    {
        public virtual decimal Amount { get; set; }

        public override object GetModel(NHibernate.ISession session)
        {
            var model = new SpendXMoreModel { SpendAmountMore = 0 };

            var cartModel = MrCMSApplication.Get<CartModel>();
            if (cartModel == null) return model;

            if (cartModel.Subtotal > 0 && cartModel.Subtotal < Amount)
                model.SpendAmountMore = Amount - (cartModel.Subtotal + cartModel.ItemTax);

            return model;
        }
    }

    public class SpendXMoreModel
    {
        public virtual decimal SpendAmountMore { get; set; }

        public virtual bool Show
        {
            get { return SpendAmountMore > 0; }
        }

        public virtual string DisplayMessage
        {
            get
            {
                return SpendAmountMore > 0
                           ? string.Format("Spend {0} more to get free Shipping!", SpendAmountMore.ToCurrencyFormat())
                           : string.Empty;
            }
        }
    }
}
