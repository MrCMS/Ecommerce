using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Entities.Widget;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class NotWhatYouWereLookingForWidget : Widget
    {
        public override object GetModel(NHibernate.ISession session)
        {
            //Don't show if not on a product page
            var product = CurrentRequestData.CurrentPage as Product;
            if (product == null)
                return new List<Category>();

            //Don't show if not redirected from another site
            var requestBase = MrCMSApplication.Get<HttpRequestBase>();
            var urlReferrer = requestBase.UrlReferrer;
            if (urlReferrer == null || urlReferrer.Authority != CurrentRequestData.CurrentSite.BaseUrl)
                return product.Categories;

            return new List<Category>();
        }
    }
}