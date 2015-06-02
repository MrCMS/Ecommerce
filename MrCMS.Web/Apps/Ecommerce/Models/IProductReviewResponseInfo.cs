namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public interface IProductReviewResponseInfo
    {
        ProductReviewResponseType Type { get; }
        string Message { get; }
        string RedirectUrl { get; }
    }
}