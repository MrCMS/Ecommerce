using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class UserAccountAddressesController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IUniquePageService _uniquePageService;
        private readonly IGetUserAddresses _getUserAddresses;

        public UserAccountAddressesController(IUniquePageService uniquePageService, IGetUserAddresses getUserAddresses)
        {
            _uniquePageService = uniquePageService;
            _getUserAddresses = getUserAddresses;
        }

        public ActionResult Show(UserAccountAddresses page)
        {
            User user = CurrentRequestData.CurrentUser;
            if (user == null)
                _uniquePageService.RedirectTo<LoginPage>();

            ViewData["addresses"] = _getUserAddresses.Get(user);

            return View(page);
        }
    }

    public interface IGetUserAddresses
    {
        IList<Address> Get(User user);
    }

    public class GetUserAddresses : IGetUserAddresses
    {
        private readonly ISession _session;

        public GetUserAddresses(ISession session)
        {
            _session = session;
        }

        public IList<Address> Get(User user)
        {
            return _session.QueryOver<Address>().OrderBy(x => x.Id).Desc.Cacheable().List();
        }
    }
}