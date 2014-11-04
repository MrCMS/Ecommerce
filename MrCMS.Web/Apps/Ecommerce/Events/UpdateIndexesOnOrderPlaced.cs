using MrCMS.Tasks;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using MrCMS.Website;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class UpdateIndexesOnOrderPlaced : IOnOrderPlaced
    {
        public void Execute(OrderPlacedArgs args)
        {
            CurrentRequestData.OnEndRequest.Add(kernel => kernel.Get<ITaskRunner>().ExecuteLuceneTasks());
        }
    }
}