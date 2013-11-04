using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.BackInStockNotification;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class BackInStockNotificationService : IBackInStockNotificationService
    {
        private readonly ISession _session;

        public BackInStockNotificationService(ISession session)
        {
            _session = session;
        }

        public void SaveRequest(BackInStockNotificationRequest request)
        {
            _session.Transact(session => session.Save(request));
        }
    }
}