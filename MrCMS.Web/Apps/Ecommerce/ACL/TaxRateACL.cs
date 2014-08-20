using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class TaxRateACL : ACLRule
    {
        public const string List = "List";
        public const string Add = "Add";
        public const string Edit = "Edit";
        public const string Delete = "Delete";
        public const string Settings = "Settings";

        public override string DisplayName
        {
            get { return "Tax Rate"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { List, Add, Edit, Delete, Settings };
        }
    }
}