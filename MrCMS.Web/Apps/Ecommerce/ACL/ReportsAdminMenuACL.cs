using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class ReportsAdminMenuACL : ACLRule
    {
        public const string ShowMenu = "Show Menu";

        public override string DisplayName
        {
            get { return "Reports Admin Menu"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string>
            {
                ShowMenu
            };
        }
    }
}