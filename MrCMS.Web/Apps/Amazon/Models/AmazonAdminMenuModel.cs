using System.Collections.Generic;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public class AmazonAdminMenuModel : IAdminMenuItem
    {
        private SubMenu _children;
        public string Text { get { return "e-Amazon"; } }
        public string Url { get; private set; }
        public bool CanShow { get { return true; } }
        public SubMenu Children
        {
            get
            {
                return _children ??
                    (_children = new SubMenu
                    {
                        {
                            "Admin",
                            new List<ChildMenuItem>
                            {
                                new ChildMenuItem("Dashboard", "/Admin/Apps/Amazon/App/Dashboard"),
                                new ChildMenuItem("Listings", "/Admin/Apps/Amazon/ListingGroup"),
                                new ChildMenuItem("Orders", "/Admin/Apps/Amazon/Orders")
                            }
                        },
                        {
                            "Sync",
                            new List<ChildMenuItem>
                            {
                                new ChildMenuItem("Orders", "/Admin/Apps/Amazon/Orders/SyncMany")
                            }
                        },
                        {
                            "Settings",
                            new List<ChildMenuItem>
                            {
                                new ChildMenuItem("App", "/Admin/Apps/Amazon/Settings/App"),
                                new ChildMenuItem("Seller", "/Admin/Apps/Amazon/Settings/Seller"),
                                new ChildMenuItem("Sync", "/Admin/Apps/Amazon/Settings/Sync")
                            }
                        },
                        {
                            "",
                            new List<ChildMenuItem>
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