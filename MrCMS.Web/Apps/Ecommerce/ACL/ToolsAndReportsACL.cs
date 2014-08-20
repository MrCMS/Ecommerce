using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class ToolsAndReportsACL : ACLRule
    {
        public const string Show = "Show";

        public override string DisplayName
        {
            get { return "Tools And Reports"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { Show };
        }
    }
}