using System.Collections.Generic;
using MrCMS.Web.Apps.Core.Models.Navigation;

namespace MrCMS.Web.Apps.Ecommerce.Services.UserAccount
{
    public interface IGetUserAccountLinks
    {
        IList<NavigationRecord> Get();
    }
}