using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class ProductOptionSortingACL : ACLRule
    {
        public const string List = "List";
        public const string Sort = "Sort";

        public override string DisplayName
        {
            get { return "Product Option Sorting"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { List, Sort };
        }
    }
}