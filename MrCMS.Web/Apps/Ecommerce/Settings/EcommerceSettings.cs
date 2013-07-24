using System.Web.Mvc;
using MrCMS.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;
using MrCMS.Web.Apps.Ecommerce.Entities.Currencies;

namespace MrCMS.Web.Apps.Ecommerce.Settings
{
    public class EcommerceSettings : SiteSettingsBase
    {
        [DisplayName("Page size for Administration")]
        public int PageSizeAdmin { get; set; }
        [DisplayName("Search Products per Page")]
        public string SearchProductsPerPage { get; set; }
        [DisplayName("Previous Price text")]
        public string PreviousPriceText { get; set; }

        [DisplayName("Default no image")]
        public string DefaultNoProductImage { get; set; }

        public IEnumerable<int> ProductPerPageOptions
        {
            get
            {
                return SearchProductsPerPage.Split(',').Where(s =>
                                                                  {
                                                                      int result;
                                                                      return int.TryParse(s, out result);
                                                                  }).Select(s => Convert.ToInt32(s));
            }
        }
        public IEnumerable<SelectListItem> ProductPerPageOptionItems
        {
            get
            {
                return ProductPerPageOptions.BuildSelectItemList(i => string.Format("{0} products per page", i), i => i.ToString(),
                                                   emptyItem: null);

            }
        }

        public override bool RenderInSettings
        {
            get { return false; }
        }

        [DisplayName("Site Currency")]
        public int CurrencyId { get; set; }

        public Currency Currency { get { return MrCMSApplication.Get<ISession>().Get<Currency>(CurrencyId); } }

        
    }
}