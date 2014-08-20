using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class SalesByDayACL : ACLRule
    {
        public const string View = "View";

        public override string DisplayName
        {
            get { return "Sales By Day"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { View };
        }
    }
}