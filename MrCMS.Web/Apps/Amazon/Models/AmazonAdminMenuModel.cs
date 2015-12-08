using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Web.Apps.Amazon.ACL;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public class AmazonAdminMenuModel : IAdminMenuItem
    {
        private SubMenu _children;
        public string Text { get { return "Amazon"; } }
        public string IconClass { get { return "fa fa-exchange"; } }
        public string Url { get; private set; }
        public bool CanShow { get { return CurrentRequestData.CurrentUser.CanAccess<AmazonAdminMenuACL>(AmazonAdminMenuACL.Show); } }
        public SubMenu Children
        {
            get
            {
                return _children ??
                    (_children = new SubMenu
                    {
                        
                            new ChildMenuItem("Dashboard", "/Admin/Apps/Amazon/App/Dashboard"),
                            new ChildMenuItem("Listings", "/Admin/Apps/Amazon/ListingGroup"),
                            new ChildMenuItem("Orders", "/Admin/Apps/Amazon/Orders"),
                            new ChildMenuItem("Order Sync", "/Admin/Apps/Amazon/Orders/SyncMany"),
                            new ChildMenuItem("App", "/Admin/Apps/Amazon/Settings/App"),
                            new ChildMenuItem("Seller", "/Admin/Apps/Amazon/Settings/Seller"),
                            new ChildMenuItem("Sync", "/Admin/Apps/Amazon/Settings/Sync"),
                            new ChildMenuItem("Logs", "/Admin/Apps/Amazon/Logs"),
                    });
            }
        }
        public int DisplayOrder
        {
            get { return 53; }
        }
    }
}