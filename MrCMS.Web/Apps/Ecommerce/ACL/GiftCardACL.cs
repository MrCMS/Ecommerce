using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class GiftCardACL : ACLRule
    {
        public const string List = "List";

        public override string DisplayName
        {
            get { return "Gift Card"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { List };
        }
    }
}