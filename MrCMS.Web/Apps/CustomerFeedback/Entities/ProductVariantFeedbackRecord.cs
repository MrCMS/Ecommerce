using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.CustomerFeedback.Entities
{
    public class ProductVariantFeedbackRecord : Feedback
    {
        public virtual ProductVariant ProductVariant { get; set; }
    }
}