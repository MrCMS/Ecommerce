using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Models.UserAccount;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.UserAccount
{
    public class UpdatePasswordService : IUpdatePasswordService
    {
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly ISession _session;
        private readonly IStringResourceProvider _stringResourceProvider;

        public UpdatePasswordService(IPasswordManagementService passwordManagementService, 
            ISession session, IStringResourceProvider stringResourceProvider)
        {
            _passwordManagementService = passwordManagementService;
            _session = session;
            _stringResourceProvider = stringResourceProvider;
        }

        public UpdatePasswordResult Update(UpdatePasswordModel model)
        {
            var user = CurrentRequestData.CurrentUser;
            // Check old password
            if (!_passwordManagementService.ValidateUser(user, model.CurrentPassword))
            {
                return new UpdatePasswordResult
                {
                    Success = false, 
                    Message = _stringResourceProvider.GetValue("Incorrect Current Password", "Current Password is incorrect.")
                };
            }
            
            // Change password
            _passwordManagementService.SetPassword(user, model.NewPassword, model.ConfirmNewPassword);
            _session.Transact(session => session.Update(user));

            return new UpdatePasswordResult
            {
                Success = true, 
                Message = _stringResourceProvider.GetValue("User Password Updated", "Password successfully changed.")
            };
        }
    }
}