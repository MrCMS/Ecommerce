using System;
using System.Collections.Generic;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.BackInStockNotification;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates.TokenProviders
{
    public class BackInStockTokenProvider : ITokenProvider<BackInStockNotificationRequest>
    {
        private IDictionary<string, Func<BackInStockNotificationRequest, string>> _tokens;
        public IDictionary<string, Func<BackInStockNotificationRequest, string>> Tokens { get { return _tokens = _tokens ?? GetTokens(); } }
        private IDictionary<string, Func<BackInStockNotificationRequest, string>> GetTokens()
        {
            return new Dictionary<string, Func<BackInStockNotificationRequest, string>>
                {
                    {"ProductUrl", request => request.ProductVariant.Product != null ? request.ProductVariant.Product.AbsoluteUrl  : null},
                    {"Name", request => request.ProductVariant.Product.Name}
                };
        }
    }
}