using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class AddOrderCancelledNote : IOnOrderCancelled
    {
        private readonly IOrderNoteService _orderNoteService;
        private readonly IGetCurrentUser _getCurrentUser;

        public AddOrderCancelledNote(IOrderNoteService orderNoteService, IGetCurrentUser getCurrentUser)
        {
            _orderNoteService = orderNoteService;
            _getCurrentUser = getCurrentUser;
        }

        public void Execute(OrderCancelledArgs args)
        {
            var user = _getCurrentUser.Get();
            _orderNoteService.AddOrderNoteAudit(string.Format("Order marked as cancelled by {0}.", user != null ? user.Name : "System"), args.Order);
        }
    }
}