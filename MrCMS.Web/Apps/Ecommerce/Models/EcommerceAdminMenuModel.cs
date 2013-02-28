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
                            new ChildMenuItem("Products", "/Admin/Product"),
                        });
            }
        }
        public int DisplayOrder { get { return 50; } }
    }
}