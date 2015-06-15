using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;

namespace MrCMS.Web.Apps.Ecommerce.Services.UserAccount
{
    public interface IGetUserAddresses
    {
        IList<Address> Get(User user);
    }
}