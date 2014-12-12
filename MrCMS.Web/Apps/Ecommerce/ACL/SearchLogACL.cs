using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class SearchLogACL : ACLRule
    {
        public const string List = "List";
        public const string Edit = "Edit";
        public const string Delete = "Delete";
        public override string DisplayName
        {
            get { return "Search Log"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string>{ List,Edit,Delete};
        }
    }
}