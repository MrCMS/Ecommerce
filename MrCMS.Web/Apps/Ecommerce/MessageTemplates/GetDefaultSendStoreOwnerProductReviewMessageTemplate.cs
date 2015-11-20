using System;
using MrCMS.Messages;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates
{
    public class GetDefaultSendStoreOwnerProductReviewMessageTemplate: GetDefaultTemplate<ProductReviewMessageTemplate>
    {
        public override ProductReviewMessageTemplate Get()
        {
            return new ProductReviewMessageTemplate
            {
                FromName = CurrentRequestData.CurrentSite.Name,
                ToAddress = "you@example.com",
                ToName = "Site Owner",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Product Review", CurrentRequestData.CurrentSite.Name),
                Body = "<p>The product {Name} just received a new review.</p><p>{ProductUrl}</p><p>Review Title: {Title}</p><p>Message: {Text}</p>",
                IsHtml = true
            };
        }
    }
}