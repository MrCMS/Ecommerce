using MrCMS.Helpers;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
using NHibernate;

namespace MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Services
{
    public class CustomerInteractionAdminService : ICustomerInteractionAdminService
    {
        private readonly ISession _session;

        public CustomerInteractionAdminService(ISession session)
        {
            _session = session;
        }

        public void Add(CorrespondenceRecord record)
        {
            _session.Transact(session => session.Save(record));
        }
    }
}