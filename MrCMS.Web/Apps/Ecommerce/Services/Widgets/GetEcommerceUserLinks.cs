using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.UserAccount;
using MrCMS.Web.Apps.Ecommerce.Widgets;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.Widgets
{
    public class GetEcommerceUserLinks : GetWidgetModelBase<EcommerceUserLinks>
    {
        private readonly IUniquePageService _uniquePageService;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly IGetUserAccountLinks _getUserAccountLinks;

        public GetEcommerceUserLinks(IUniquePageService uniquePageService, 
            IStringResourceProvider stringResourceProvider, IGetUserAccountLinks getUserAccountLinks)
        {
            _uniquePageService = uniquePageService;
            _stringResourceProvider = stringResourceProvider;
            _getUserAccountLinks = getUserAccountLinks;
        }

        public override object GetModel(EcommerceUserLinks widget)
        {
            var navigationRecords = new List<NavigationRecord>();
            var loggedIn = CurrentRequestData.CurrentUser != null;
            if (loggedIn)
            {
                navigationRecords.AddRange(_getUserAccountLinks.Get());

                navigationRecords.Add(new NavigationRecord
                {
                    Text = MvcHtmlString.Create(_stringResourceProvider.GetValue("Logout")),
                    Url = MvcHtmlString.Create(string.Format("/logout"))
                });
            }
            else
            {
                var loginPageLiveUrlSegment = _uniquePageService.GetUniquePage<LoginPage>().LiveUrlSegment;
                if (loginPageLiveUrlSegment != null)
                {
                    navigationRecords.Add(new NavigationRecord
                    {
                        Text = MvcHtmlString.Create(_stringResourceProvider.GetValue("Login")),
                        Url = MvcHtmlString.Create(string.Format("/{0}", loginPageLiveUrlSegment))
                    });
                }
                var registerPageLiveUrlSegment = _uniquePageService.GetUniquePage<RegisterPage>().LiveUrlSegment;
                if (registerPageLiveUrlSegment != null)
                {
                    navigationRecords.Add(new NavigationRecord
                    {
                        Text = MvcHtmlString.Create(_stringResourceProvider.GetValue("Register")),
                        Url = MvcHtmlString.Create(string.Format("/{0}", registerPageLiveUrlSegment))
                    });
                }
            }

            return navigationRecords;
        }
    }
}