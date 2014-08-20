using MrCMS.Events;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class AddOrderDeletedNote :IOnDeleted<Order>
    {
        private readonly IOrderNoteService _orderNoteService;
        private readonly IGetCurrentUser _getCurrentUser;

        public AddOrderDeletedNote(IOrderNoteService orderNoteService,IGetCurrentUser getCurrentUser)
        {
            _orderNoteService = orderNoteService;
            _getCurrentUser = getCurrentUser;
        }

        public void Execute(OnDeletedArgs<Order> args)
        {
            _orderNoteService.AddOrderNoteAudit(string.Format("Order marked as deleted by {0}.",
                _getCurrentUser.Get().Name), args.Item);
        }
    }
}