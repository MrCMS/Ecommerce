using MrCMS.Web.Apps.Ecommerce.Entities.Users;

namespace MrCMS.Web.Apps.Ecommerce.Services.UserAccount
{
    public interface IExistingAddressService
    {
        void Update(Address address);
        void Delete(Address address);
    }
}