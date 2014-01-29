using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class CartWidget : Widget
    {
        public override object GetModel(NHibernate.ISession session)
        {
            return MrCMSApplication.Get<CartModel>();
        }
    }
}