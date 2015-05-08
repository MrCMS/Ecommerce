using System.Collections.Generic;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.CustomerFeedback.Services
{
    public interface IGetCustomerInteraction
    {
        IList<CorrespondenceRecord> Get(Order order);
    }
}