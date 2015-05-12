using MrCMS.Entities.Multisite;
using MrCMS.Messages;
using MrCMS.Web.Apps.CustomerFeedback.Entities;

namespace MrCMS.Web.Apps.CustomerFeedback.MessageTemplates
{
    public class CustomerServiceReplyMessageTemplate : MessageTemplate<CorrespondenceRecord>
    {
    }

    public class GetDefaultCustomerServiceReplyMessageTemplate : GetDefaultTemplate<CustomerServiceReplyMessageTemplate>
    {
        private readonly Site _site;

        public GetDefaultCustomerServiceReplyMessageTemplate(Site site)
        {
            _site = site;
        }

        public override CustomerServiceReplyMessageTemplate Get()
        {
            var fromName = _site.Name;
            return new CustomerServiceReplyMessageTemplate
            {
                FromName = fromName,
                FromAddress = "",
                ToName = "",
                ToAddress = "",
                Cc = string.Empty,
                Bcc = string.Empty,
                Subject = "Customer Service Reply",
                Body = "<p></p>",
                IsHtml = true
            };
        }
    }
}