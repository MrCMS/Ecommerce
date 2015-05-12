using MrCMS.Helpers;
using MrCMS.Web.Apps.CustomerFeedback.Controllers;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
using MrCMS.Web.Apps.CustomerFeedback.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.CustomerFeedback.Services
{
    public class CustomerInteractionService : ICustomerInteractionService
    {
        private readonly ISession _session;

        public CustomerInteractionService(ISession session)
        {
            _session = session;
        }

        public void Add(CustomerInteractionPostModel model)
        {
            var correspondenceRecord = new CorrespondenceRecord
            {
                CorrespondenceDirection = CorrespondenceDirection.Incoming,
                Order = model.Order,
                MessageInfo = model.Message,
                User = CurrentRequestData.CurrentUser ?? CurrentRequestData.CurrentUser
            };

            _session.Transact(session => session.Save(correspondenceRecord));
        }
    }
}