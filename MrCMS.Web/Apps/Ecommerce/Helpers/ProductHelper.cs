using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class ProductHelper
    {
        public static IList<PriceBreak> GetPriceBreaks(this Product product)
        {

            return MrCMSApplication.Get<ISession>()
                                   .QueryOver<PriceBreak>()
                                   .Where(@break => @break.Item == product)
                                   .Cacheable()
                                   .List();
        }

        public static IList<PriceBreak> GetPriceBreaks(this ProductVariant variant)
        {
            return MrCMSApplication.Get<ISession>()
                                   .QueryOver<PriceBreak>()
                                   .Where(@break => @break.Item == variant)
                                   .Cacheable()
                                   .List();
        }
    }
}