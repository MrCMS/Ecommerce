using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IOrderNoteService
    {
        IList<OrderNote> GetAll();
        void Save(OrderNote item);
        void Delete(OrderNote item);
    }
}