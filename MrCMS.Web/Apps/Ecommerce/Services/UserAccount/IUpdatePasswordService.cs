using MrCMS.Web.Apps.Ecommerce.Models.UserAccount;

namespace MrCMS.Web.Apps.Ecommerce.Services.UserAccount
{
    public interface IUpdatePasswordService
    {
        UpdatePasswordResult Update(UpdatePasswordModel model);
    }
}