using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class CategoryACL : ACLRule
    {
        public const string List = "List";

        public override string DisplayName
        {
            get { return "Category"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { List };
        }
    }
}