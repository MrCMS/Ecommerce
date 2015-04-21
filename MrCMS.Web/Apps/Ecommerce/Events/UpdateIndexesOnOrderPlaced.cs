using MrCMS.Tasks;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using MrCMS.Website;
using MrCMS.Website.Filters;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class UpdateIndexesOnOrderPlaced : IOnOrderPlaced
    {
        public void Execute(OrderPlacedArgs args)
        {
            CurrentRequestData.OnEndRequest.Add(new ExecuteLuceneTasks());
        }
    }
}