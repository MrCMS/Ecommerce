using System.Threading.Tasks;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models.RegisterAndLogin;
using MrCMS.Web.Apps.Core.Services;
using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderPlacedService : IOrderPlacedService
    {
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;
        private readonly ISession _session;
        private readonly IRegistrationService _registrationService;

        public OrderPlacedService(IUserService userService, ILoginService loginService, ISession session, IRegistrationService registrationService)
        {
            _userService = userService;
            _loginService = loginService;
            _session = session;
            _registrationService = registrationService;
        }

        public EmailRegistrationStatus GetRegistrationStatus(string orderEmail)
        {
            if (CurrentRequestData.CurrentUser != null)
                return EmailRegistrationStatus.LoggedIn;
            if (_userService.GetUserByEmail(orderEmail) != null)
                return EmailRegistrationStatus.EmailInUse;
            return EmailRegistrationStatus.Available;
        }

        public async Task<LoginAndAssociateOrderResult> LoginAndAssociateOrder(LoginModel model, Order order)
        {
            var authenticateUser = await _loginService.AuthenticateUser(model);
            if (!authenticateUser.Success)
                return new LoginAndAssociateOrderResult { Error = "We were unable to log you in, please check your password and try again" };

            order.User = CurrentRequestData.CurrentUser;
            _session.Transact(session => session.Update(order));
            return new LoginAndAssociateOrderResult();
        }

        public async Task<RegisterAndAssociateOrderResult> RegisterAndAssociateOrder(RegisterModel model, Order order)
        {
            if (!_registrationService.CheckEmailIsNotRegistered(model.Email))
                return new RegisterAndAssociateOrderResult { Error = "The provided email already has an account associated" };

            var registeredUser = await _registrationService.RegisterUser(model);
            order.User = registeredUser;
            _session.Transact(session => session.Update(order));
            return new RegisterAndAssociateOrderResult();
        }
    }
}