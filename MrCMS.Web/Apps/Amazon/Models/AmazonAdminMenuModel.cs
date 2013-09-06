using System.Collections.Generic;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public class AmazonAdminMenuModel : IAdminMenuItem
    {
        private IDictionary<string, List<IMenuItem>> _children;
        public string Text { get { return "Amazon"; } }
        public string Url { get; private set; }
        public bool CanShow { get { return true; } }
        public IDictionary<string, List<IMenuItem>> Children
        {
            get
            {
                return _children ?? 
                    (_children = new Dictionary<string, List<IMenuItem>>
                    {
                        {
                            "Admin",
                            new List<IMenuItem>
                            {
                                new ChildMenuItem("Dashboard", "/Admin/Apps/Amazon/App/Dashboard"),
                                new ChildMenuItem("Listings", "/Admin/Apps/Amazon/Listing"),
                                new ChildMenuItem("Orders", "/Admin/Apps/Amazon/Orders"),
                            }
                        },
                        {
                            "Settings",
                            new List<IMenuItem>
                            {
                                new ChildMenuItem("App", "/Admin/Apps/Amazon/App/Settings"),
                                new ChildMenuItem("Seller", "/Admin/Apps/Amazon/Seller/Settings")
                            }
                        },
                        {
                            "Auditing",
                            new List<IMenuItem>
                            {
                                new ChildMenuItem("Logs", "/Admin/Apps/Amazon/Log"),
                            }
                        }
                    });
            }
        }
        public int DisplayOrder
        {
            get { return 51; }
        }
    }
}