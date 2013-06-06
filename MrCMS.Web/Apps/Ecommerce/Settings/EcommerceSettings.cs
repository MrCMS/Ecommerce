using MrCMS.Entities;
using MrCMS.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MrCMS.Web.Apps.Ecommerce.Settings
{
    public class EcommerceSettings : SiteSettingsBase
    {
        [DisplayName("Page size for Administration")]
        public int PageSizeAdmin { get; set; }
        [DisplayName("Category Products per Page")]
        public string CategoryProductsPerPage { get; set; }

        public override bool RenderInSettings
        {
            get { return false; }
        }
    }
}