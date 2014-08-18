using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IGiftCardAdminService
    {
        IPagedList<GiftCard> Search(GiftCardSearchQuery query);
        List<SelectListItem> GetTypeOptions();
        List<SelectListItem> GetStatusOptions();
        void Add(GiftCard giftCard);
        void Update(GiftCard giftCard);
        string GenerateCode();
    }
}