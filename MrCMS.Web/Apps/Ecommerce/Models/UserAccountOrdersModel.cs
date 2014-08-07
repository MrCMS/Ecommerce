using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Areas.Admin.Models;

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