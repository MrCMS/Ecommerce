using System;
using System.Collections.Generic;
using MrCMS.Services;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
using MrCMS.Web.Apps.CustomerFeedback.Pages;

namespace MrCMS.Web.Apps.CustomerFeedback.MessageTemplates.TokenProviders
{
    public class FeedbackRecordTokenProvider : ITokenProvider<FeedbackRecord>
    {
        private readonly IUniquePageService _uniquePageService;

        public FeedbackRecordTokenProvider(IUniquePageService uniquePageService)
        {
            _uniquePageService = uniquePageService;
        }

        private IDictionary<string, Func<FeedbackRecord, string>> _tokens;
        public IDictionary<string, Func<FeedbackRecord, string>> Tokens { get { return _tokens = _tokens ?? GetTokens(); } }

        private IDictionary<string, Func<FeedbackRecord, string>> GetTokens()
        {
            return new Dictionary<string, Func<FeedbackRecord, string>>
            {
                {
                    "OrderGuid",
                    i => i.Order != null ? i.Order.Guid.ToString() : string.Empty
                },
                {
                    "OrderEmail",
                    i => i.Order != null ? i.Order.OrderEmail : string.Empty
                },
                {
                    "OrderName",
                    i => i.Order != null ? i.Order.BillingAddress.Name : string.Empty
                },
                {
                    "OrderId",
                    i => i.Order != null ? i.Order.Id.ToString() : string.Empty
                },
                {
                    "FeedbackUrl",
                    i => i.Order != null ? string.Format("{0}?guid={1}", _uniquePageService.GetUniquePage<OrderFeedback>().AbsoluteUrl, i.Order.Guid) : string.Empty
                }
            };
        }
    }
}