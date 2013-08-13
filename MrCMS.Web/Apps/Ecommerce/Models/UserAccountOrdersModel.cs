using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class UserAccountOrdersModel : AsyncListModel<Order>
    {
        public UserAccountOrdersModel(PagedList<Order> items, int id)
            : base(items, id)
        {
        }
    }
}