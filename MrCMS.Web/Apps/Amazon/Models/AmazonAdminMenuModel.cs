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

        public bool CanShow { get { return CurrentRequestData.CurrentUser.CanAccess<AmazonAdminMenuACL>(AmazonAdminMenuACL.ShowMenu); } }

        public SubMenu Children
        {
            get
            {
                return _children ?? (_children = new SubMenu
                {
                        new ChildMenuItem("Dashboard", "/Admin/Apps/Amazon/App/Dashboard", ACLOption.Create(new AmazonAdminMenuACL(), AmazonAdminMenuACL.Dashboard)),
                        new ChildMenuItem("Listings", "/Admin/Apps/Amazon/ListingGroup", ACLOption.Create(new AmazonAdminMenuACL(), AmazonAdminMenuACL.Listings)),
                        new ChildMenuItem("Orders", "/Admin/Apps/Amazon/Orders", ACLOption.Create(new AmazonAdminMenuACL(), AmazonAdminMenuACL.Orders)),
                        new ChildMenuItem("Order Sync", "/Admin/Apps/Amazon/Orders/SyncMany", ACLOption.Create(new AmazonAdminMenuACL(), AmazonAdminMenuACL.OrderSync)),
                        new ChildMenuItem("App", "/Admin/Apps/Amazon/Settings/App", ACLOption.Create(new AmazonAdminMenuACL(), AmazonAdminMenuACL.AmazonApp)),
                        new ChildMenuItem("Seller", "/Admin/Apps/Amazon/Settings/Seller", ACLOption.Create(new AmazonAdminMenuACL(), AmazonAdminMenuACL.Seller)),
                        new ChildMenuItem("Sync", "/Admin/Apps/Amazon/Settings/Sync", ACLOption.Create(new AmazonAdminMenuACL(), AmazonAdminMenuACL.Sync)),
                        new ChildMenuItem("Logs", "/Admin/Apps/Amazon/Logs", ACLOption.Create(new AmazonAdminMenuACL(), AmazonAdminMenuACL.Logs)),
                });
            }
        }

        public int DisplayOrder
        {
            get { return 53; }
        }
    }
}