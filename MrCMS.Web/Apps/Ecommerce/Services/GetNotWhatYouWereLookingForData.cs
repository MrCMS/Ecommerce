using System;
using System.Collections.Generic;
using System.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Widgets;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetNotWhatYouWereLookingForData : GetWidgetModelBase<NotWhatYouWereLookingForWidget>
    {
        private readonly HttpRequestBase _request;
        private readonly Site _site;

        public GetNotWhatYouWereLookingForData(HttpRequestBase request, Site site)
        {
            _request = request;
            _site = site;
        }

        public override object GetModel(NotWhatYouWereLookingForWidget widget)
        {
            //Don't show if not on a product page
            var product = CurrentRequestData.CurrentPage as Product;
            if (product == null)
                return new List<Category>();

            //Don't show if not redirected from another site
            Uri urlReferrer = _request.UrlReferrer;
            if (urlReferrer == null || urlReferrer.Authority != _site.BaseUrl)
                return product.Categories;

            return new List<Category>();
        }
    }
}