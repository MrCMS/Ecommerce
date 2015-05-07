using MrCMS.Entities.Multisite;
using MrCMS.Messages;

namespace MrCMS.Web.Apps.CustomerFeedback.MessageTemplates
{
    public class GetDefaultCustomerFeedbackMessageTemplate : GetDefaultTemplate<CustomerFeedbackMessageTemplate>
    {
        private readonly Site _site;

        public GetDefaultCustomerFeedbackMessageTemplate(Site site)
        {
            _site = site;
        }

        public override CustomerFeedbackMessageTemplate Get()
        {
            var fromName = _site.Name;
            return new CustomerFeedbackMessageTemplate
            {
                FromName = fromName,
                ToName = "{OrderName}",
                ToAddress = "{OrderEmail}",
                Bcc = string.Empty,
                Cc = string.Empty,
                Subject = "Order Feedback",
                Body = "<p>Dear {OrderName}</p><p><a href=\"{FeedbackUrl}\">Leave Feedback</a></p>",
                IsHtml = true
            };
        }
    }
}