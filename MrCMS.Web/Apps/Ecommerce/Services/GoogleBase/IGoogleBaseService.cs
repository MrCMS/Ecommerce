using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.GoogleBase
{
    public interface IGoogleBaseService
    {
        List<SelectListItem> GetGoogleCategories();
        GoogleBaseProduct GetGoogleBaseProduct(int id);
        void AddGoogleBaseProduct(GoogleBaseProduct item);
        void UpdateGoogleBaseProduct(GoogleBaseProduct item);
        void UpdateGoogleBaseProductAndVariant(ProductVariant productVariant, GoogleBaseProduct googleBaseProduct);
    }
}