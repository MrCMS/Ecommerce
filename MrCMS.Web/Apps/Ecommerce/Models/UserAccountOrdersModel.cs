using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class UserAccountOrdersModel : AsyncListModel<Order>
    {
        public UserAccountOrdersModel(IPagedList<Order> items, int id)
            : base(items, id)
        {
        }
    }
}