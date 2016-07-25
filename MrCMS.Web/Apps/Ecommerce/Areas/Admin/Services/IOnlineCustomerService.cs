using System;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IOnlineCustomerService
    {
        IPagedList<OnlineCustomerCart> Search(OnlineCustomerSearchQuery query);
        OnlineCustomerCart GetCart(Guid userGuid);
    }
}