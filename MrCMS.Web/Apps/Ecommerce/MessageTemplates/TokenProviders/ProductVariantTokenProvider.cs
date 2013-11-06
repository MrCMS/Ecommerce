using System;
using System.Collections.Generic;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates.TokenProviders
{
    public class ProductVariantTokenProvider : ITokenProvider<ProductVariant>
    {
        private IDictionary<string, Func<ProductVariant, string>> _tokens;
        public IDictionary<string, Func<ProductVariant, string>> Tokens { get { return _tokens = _tokens ?? GetTokens(); } }
        private IDictionary<string, Func<ProductVariant, string>> GetTokens()
        {
            return new Dictionary<string, Func<ProductVariant, string>>
                {
                    {"ProductUrl", variant => variant.Product != null ? variant.Product.AbsoluteUrl : null}
                };
        }
    }
}