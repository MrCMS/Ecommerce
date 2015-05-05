using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Services.UserAccount
{
    public class GetUserAccountLinks : IGetUserAccountLinks
    {
        private readonly IUniquePageService _uniquePageService;
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly ProductReviewSettings _productReviewSettings;

        public GetUserAccountLinks(IUniquePageService uniquePageService, 
            EcommerceSettings ecommerceSettings, ProductReviewSettings productReviewSettings)
        {
            _uniquePageService = uniquePageService;
            _ecommerceSettings = ecommerceSettings;
            _productReviewSettings = productReviewSettings;
        }

        public IList<NavigationRecord> Get()
        {

            var list = new List<NavigationRecord>{};

            var pagesToAdd = GetPages();

            list.AddRange(pagesToAdd.Select(webpage => new NavigationRecord
            {
                Text = MvcHtmlString.Create(webpage.Name), Url = MvcHtmlString.Create("/" + webpage.LiveUrlSegment)
            }));

            return list;
        }

        private List<Webpage> GetPages()
        {
            // Try and get them all
            var pages = new List<Webpage>
            {
                _uniquePageService.GetUniquePage<UserAccountInfo>(),
                _uniquePageService.GetUniquePage<UserAccountAddresses>(),
                _uniquePageService.GetUniquePage<UserAccountChangePassword>(),
                _uniquePageService.GetUniquePage<UserAccountOrders>(),
                _uniquePageService.GetUniquePage<UserAccountReviews>(),
                _uniquePageService.GetUniquePage<UserAccountRewardPoints>()
            };

            // Filter out ones that haven't been added
            return pages.FindAll(webpage => webpage != null);
        }
    }
}