using System;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IOnlineCustomerService
    {
        IPagedList<OnlineCustomerCart> Search(OnlineCustomerSearchQuery query);
        OnlineCustomerCart GetCart(Guid userGuid);
    }
}