using MrCMS.Entities.Multisite;
using MrCMS.Messages;

namespace MrCMS.Web.Apps.CustomerFeedback.MessageTemplates
{
    public class GetDefaultInitialCustomerServiceMessageTemplate : GetDefaultTemplate<InitialCustomerServiceMessageTemplate>
    {
        private readonly Site _site;

        public GetDefaultInitialCustomerServiceMessageTemplate(Site site)
        {
            _site = site;
        }


        public override InitialCustomerServiceMessageTemplate Get()
        {
            var fromName = _site.Name;

            return new InitialCustomerServiceMessageTemplate
            {
                FromName = fromName,
                FromAddress = "admin@yoursite.com",
                ToName = "{OrderName}",
                ToAddress = "{OrderEmail}",
                Cc = string.Empty,
                Bcc = string.Empty,
                Subject = "Customer Services - Feedback - Order {OrderId}",
                Body = "{MessageInfo}<p>View more information by clicking <a href='{InteractionPageUrl}'>here</a></p>",
                IsHtml = true
            };
        }
    }
}