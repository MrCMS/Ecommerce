using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IGetUserAccountLinks
    {
        IList<NavigationRecord> Get();
    }

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
            var userAccountInfo = _uniquePageService.GetUniquePage<UserAccountInfo>();
            var userAccountAddresses = _uniquePageService.GetUniquePage<UserAccountAddresses>();
            var userAccountChangePassword = _uniquePageService.GetUniquePage<UserAccountChangePassword>();
            var userAccountOrders = _uniquePageService.GetUniquePage<UserAccountOrders>();
            var userAccountReviews = _uniquePageService.GetUniquePage<UserAccountReviews>();
            var userAccountRewards = _uniquePageService.GetUniquePage<UserAccountRewardPoints>();

            var pages = new List<Webpage>();
            if (userAccountInfo != null)
                pages.Add(userAccountInfo);
            if (userAccountChangePassword != null)
                pages.Add(userAccountChangePassword);
            if (userAccountOrders != null)
                pages.Add(userAccountOrders);
            if (userAccountAddresses != null)
                pages.Add(userAccountAddresses);
            if (userAccountReviews != null && _productReviewSettings.EnableProductReviews)
                pages.Add(userAccountReviews);
            if (userAccountRewards != null && _ecommerceSettings.RewardPointsEnabled)
                pages.Add(userAccountRewards);

            return pages;
        }
    }
}