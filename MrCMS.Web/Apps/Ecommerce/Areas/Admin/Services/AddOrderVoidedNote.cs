using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class AddOrderVoidedNote : IOnOrderVoided
    {
        private readonly IOrderNoteService _orderNoteService;
        private readonly IGetCurrentUser _getCurrentUser;

        public AddOrderVoidedNote(IOrderNoteService orderNoteService, IGetCurrentUser getCurrentUser)
        {
            _orderNoteService = orderNoteService;
            _getCurrentUser = getCurrentUser;
        }

        public void Execute(OrderVoidedArgs args)
        {
            var currentUser = _getCurrentUser.Get();
            _orderNoteService.AddOrderNoteAudit(string.Format("Order payment marked as void by {0}.",
                currentUser != null ? currentUser.Name : "System"), args.Order);
        }
    }
}