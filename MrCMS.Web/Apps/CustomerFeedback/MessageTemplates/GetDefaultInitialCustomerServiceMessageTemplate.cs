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
                FromAddress = "",
                ToName = "{OrderName}",
                ToAddress = "{OrderEmail}",
                Cc = string.Empty,
                Bcc = string.Empty,
                Subject = "Customer Services - ",
                Body = "<p></p>",
                IsHtml = true
            };
        }
    }
}