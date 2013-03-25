using System.Collections.Generic;
using MrCMS.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EcommerceAdminMenuModel : IAdminMenuItem
    {
        private List<IMenuItem> _children;
        public string Text { get { return "Ecommerce"; } }
        public string Url { get; private set; }
        public List<IMenuItem> Children
        {
            get
            {
                return _children ??
                       (_children =
                        new List<IMenuItem>
                        {
                            new ChildMenuItem("Products", "/Admin/Apps/Ecommerce/Product"),
                            new ChildMenuItem("Categories", "/Admin/Apps/Ecommerce/Category"),
                            new ChildMenuItem("Geographic Data", "/Admin/Apps/Ecommerce/Country"),
                            new ChildMenuItem("Tax Rates", "/Admin/Apps/Ecommerce/TaxRate"),
                            new ChildMenuItem("Product Specification Options", "/Admin/Apps/Ecommerce/ProductSpecificationOption"),
                        });
            }
        }
        public int DisplayOrder { get { return 50; } }
    }
}