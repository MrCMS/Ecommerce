using System;
using System.Collections.Generic;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates.TokenProviders
{
    public class ProductReviewTokenProvider : ITokenProvider<ProductReview>
    {
        private IDictionary<string, Func<ProductReview, string>> _tokens;
        public IDictionary<string, Func<ProductReview, string>> Tokens { get { return _tokens = _tokens ?? GetTokens(); } }
        private IDictionary<string, Func<ProductReview, string>> GetTokens()
        {
            return new Dictionary<string, Func<ProductReview, string>>
                {
                    {"ProductUrl", request => request.ProductVariant.Product != null ? request.ProductVariant.Product.AbsoluteUrl  : null},
                    {"Name", request => request.ProductVariant.Product.Name},
                    {"UserName",request=>request.User.Name}
                };
        }
    }
}