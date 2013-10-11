using System.Collections.Generic;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public class AmazonAdminMenuModel : IAdminMenuItem
    {
        private IDictionary<string, List<IMenuItem>> _children;
        public string Text { get { return "e-Amazon"; } }
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
                                new ChildMenuItem("Listings", "/Admin/Apps/Amazon/ListingGroup"),
                                new ChildMenuItem("Orders", "/Admin/Apps/Amazon/Orders")
                            }
                        },
                        {
                            "Sync",
                            new List<IMenuItem>
                            {
                                new ChildMenuItem("Orders", "/Admin/Apps/Amazon/Orders/SyncMany")
                            }
                        },
                        {
                            "Settings",
                            new List<IMenuItem>
                            {
                                new ChildMenuItem("App", "/Admin/Apps/Amazon/Settings/App"),
                                new ChildMenuItem("Seller", "/Admin/Apps/Amazon/Settings/Seller")
                            }
                        },
                        {
                            "Auditing",
                            new List<IMenuItem>
                            {
                                new ChildMenuItem("Logs", "/Admin/Apps/Amazon/Logs")
                            }
                        }
                    });
            }
        }
        public int DisplayOrder
        {
            get { return 53; }
        }
    }
}