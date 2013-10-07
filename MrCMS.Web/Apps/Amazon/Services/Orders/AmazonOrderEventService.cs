using MrCMS.Helpers;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Services.Orders.Events;
using Ninject;
using System.Linq;

namespace MrCMS.Web.Apps.Amazon.Services.Orders
{
    public class AmazonOrderEventService : IAmazonOrderEventService
    {
        private readonly IKernel _kernel;

        public AmazonOrderEventService(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void AmazonOrderPlaced(AmazonOrder order)
        {
            var items = GetAll<IOnAmazonOrderPlaced>();
            if (items.Any())
                items.ForEach(placed => placed.OnAmazonOrderPlaced(order));
        }

        private IOrderedEnumerable<T> GetAll<T>() where T : IAmazonOrderEvent
        {
            return _kernel.GetAll<T>().OrderBy(placed => placed.Order);
        }
    }
}