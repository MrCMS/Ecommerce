using System.Collections.Specialized;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface ISetRestrictedShippingMethods
    {
        void SetMethods(ProductVariant productVariant, NameValueCollection requestData);
    }
}