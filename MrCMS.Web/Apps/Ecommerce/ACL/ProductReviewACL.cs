using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class ProductReviewACL : ACLRule
    {
        public const string List = "List";
        public const string Edit = "Edit";
        public const string Delete = "Delete";

        public override string DisplayName
        {
            get { return "Product Reviews"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> {List, Edit, Delete};
        }
    }
}