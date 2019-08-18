using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public static class OrderExtensions
    {
        public static int GetTotalOrders(this Order order)
        {
            var session = MrCMSApplication.Get<ISession>();

            return session.QueryOver<Order>().Where(x => x.OrderEmail == order.OrderEmail).Cacheable().RowCount();
        }
    }
}