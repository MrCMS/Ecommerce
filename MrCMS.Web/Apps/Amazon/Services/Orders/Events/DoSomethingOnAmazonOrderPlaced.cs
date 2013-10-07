using MrCMS.Web.Apps.Amazon.Entities.Orders;
using NHibernate;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Events
{
    public class DoSomethingOnAmazonOrderPlaced : IOnAmazonOrderPlaced
    {
        private readonly ISession _session;

        public DoSomethingOnAmazonOrderPlaced(ISession session)
        {
            _session = session;
        }

        public int Order { get { return 1; } }
        public void OnAmazonOrderPlaced(AmazonOrder order)
        {
            //TODO
        }
    }
}