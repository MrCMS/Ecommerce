using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class UserOrdersList : Widget
    {
        public override object GetModel(NHibernate.ISession session)
        {
            var user = CurrentRequestData.CurrentUser;
            if (user != null)
            {
                var ordersByUser = MrCMSApplication.Get<OrderService>().GetOrdersByUser(user, 1);
                var model =
                    new UserAccountOrdersModel(
                        new PagedList<Order>(ordersByUser, ordersByUser.PageNumber, ordersByUser.PageSize), user.Id);
                return model;
            }
            return base.GetModel(session);
        }
    }
}