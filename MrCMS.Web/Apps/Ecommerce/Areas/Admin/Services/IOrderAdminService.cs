using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IOrderAdminService
    {
        IPagedList<Order> Search(OrderSearchModel model);
        Order Get(int id);
        void Cancel(Order order);
        List<SelectListItem> GetShippingStatusOptions();
        List<SelectListItem> GetPaymentStatusOptions();
        List<SelectListItem> GetShippingMethodOptions();
        void MarkAsShipped(Order order);
        void MarkAsPaid(Order order);
        void MarkAsVoided(Order order);
        void SetTrackingNumber(Order order);
        void Delete(Order order);
    }

}