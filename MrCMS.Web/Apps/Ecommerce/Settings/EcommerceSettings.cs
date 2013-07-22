using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Settings
{
    public class EcommerceSettings : SiteSettingsBase
    {
        [DisplayName("Page size for Administration")]
        public int PageSizeAdmin { get; set; }
        [DisplayName("Search Products per Page")]
        public string SearchProductsPerPage { get; set; }

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
    }
}