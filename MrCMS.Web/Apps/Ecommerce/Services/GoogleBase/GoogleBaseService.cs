using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using NHibernate;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Services.GoogleBase
{
    public class GoogleBaseService : IGoogleBaseService
    {
        private readonly ISession _session;
        private readonly IProductVariantService _productVariantService;
        private readonly GoogleBaseSettings _googleBaseSettings;

        public GoogleBaseService(ISession session,
            IProductVariantService productVariantService,
            GoogleBaseSettings googleBaseSettings)
        {
            _session = session;
            _productVariantService = productVariantService;
            _googleBaseSettings = googleBaseSettings;
        }

        public GoogleBaseProduct GetGoogleBaseProduct(int id)
        {
            return _session.QueryOver<GoogleBaseProduct>().Where(x => x.Id == id).SingleOrDefault();
        }

        public void SaveGoogleBaseProduct(GoogleBaseProduct item)
        {
            if (item.ProductVariant != null)
                item.ProductVariant.GoogleBaseProduct = item;
            _session.Transact(session => session.SaveOrUpdate(item));
        }


        public List<SelectListItem> GetGoogleCategories()
        {
            if (!string.IsNullOrWhiteSpace(_googleBaseSettings.GoogleBaseTaxonomyFeedUrl))
            {
                GoogleBaseTaxonomyData.GetTaxonomyData(_googleBaseSettings.GoogleBaseTaxonomyFeedUrl).BuildSelectItemList(item => item, item => item, emptyItem: null); ;
            }
            return GoogleBaseTaxonomyData.Rows.BuildSelectItemList(item => item, item => item, emptyItem: null);
        }
    }
}