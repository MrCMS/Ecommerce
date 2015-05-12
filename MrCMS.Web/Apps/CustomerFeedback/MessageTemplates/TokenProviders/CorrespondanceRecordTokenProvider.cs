using System;
using System.Collections.Generic;
using MrCMS.Services;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
using MrCMS.Web.Apps.CustomerFeedback.Pages;

namespace MrCMS.Web.Apps.CustomerFeedback.MessageTemplates.TokenProviders
{
    public class CorrespondanceRecordTokenProvider : ITokenProvider<CorrespondenceRecord>
    {
        private readonly IUniquePageService _uniquePageService;

        public CorrespondanceRecordTokenProvider(IUniquePageService uniquePageService)
        {
            _uniquePageService = uniquePageService;
        }

        private IDictionary<string, Func<CorrespondenceRecord, string>> _tokens;

        public IDictionary<string, Func<CorrespondenceRecord, string>> Tokens { get { return _tokens = _tokens ?? GetTokens(); } }

        private IDictionary<string, Func<CorrespondenceRecord, string>> GetTokens()
        {
            return new Dictionary<string, Func<CorrespondenceRecord, string>>
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
                    "InteractionPageUrl",
                    i => i.Order != null ? string.Format("{0}?id={1}", _uniquePageService.GetUniquePage<CustomerInteraction>().AbsoluteUrl, i.Order.Guid) : string.Empty
                }
            };
        }
    }
}