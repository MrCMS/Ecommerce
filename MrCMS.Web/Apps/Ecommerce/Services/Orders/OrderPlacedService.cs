using System.Threading.Tasks;
using MrCMS.Helpers;
using MrCMS.Models.Auth;
using MrCMS.Services;
using MrCMS.Services.Auth;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderPlacedService : IOrderPlacedService
    {
        private readonly ILoginService _loginService;
        private readonly IRegistrationService _registrationService;
        private readonly ISession _session;
        private readonly IUserLookup _userLookup;

        public OrderPlacedService(IUserLookup userLookup, ILoginService loginService, ISession session,
            IRegistrationService registrationService)
        {
            _userLookup = userLookup;
            _loginService = loginService;
            _session = session;
            _registrationService = registrationService;
        }

        public EmailRegistrationStatus GetRegistrationStatus(string orderEmail)
        {
            if (CurrentRequestData.CurrentUser != null)
                return EmailRegistrationStatus.LoggedIn;
            if (_userLookup.GetUserByEmail(orderEmail) != null)
                return EmailRegistrationStatus.EmailInUse;
            return EmailRegistrationStatus.Available;
        }

        public LoginAndAssociateOrderResult LoginAndAssociateOrder(LoginModel model, Order order)
        {
            var authenticateUser = _loginService.AuthenticateUser(model);
            if (authenticateUser.Status != LoginStatus.Success)
                return new LoginAndAssociateOrderResult
                {
                    Error = "We were unable to log you in, please check your password and try again"
                };
            //TODO: 2FA flow

            order.User = CurrentRequestData.CurrentUser;
            _session.Transact(session => session.Update(order));
            return new LoginAndAssociateOrderResult();
        }

        public async Task<RegisterAndAssociateOrderResult> RegisterAndAssociateOrder(RegisterModel model, Order order)
        {
            if (!_registrationService.CheckEmailIsNotRegistered(model.Email))
                return new RegisterAndAssociateOrderResult
                {
                    Error = "The provided email already has an account associated"
                };

            var registeredUser = await _registrationService.RegisterUser(model);
            order.User = registeredUser;
            _session.Transact(session => session.Update(order));
            return new RegisterAndAssociateOrderResult();
        }

        public bool UpdateAnalytics(Order order)
        {
            if (order.AnalyticsSent)
                return false;

            order.AnalyticsSent = true;
            _session.Transact(session => session.Update(order));

            return true;
        }
    }
}