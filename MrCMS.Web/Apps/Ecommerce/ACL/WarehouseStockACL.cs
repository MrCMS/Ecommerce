using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class WarehouseStockACL : ACLRule
    {
        public const string List = "List";
        public const string Edit = "Edit";

        public override string DisplayName
        {
            get { return "Warehouse Stock"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> {List, Edit};
        }
    }
}