using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class CatalogAdminMenuACL : ACLRule
    {
        public const string ShowMenu = "Show Menu";

        public override string DisplayName
        {
            get { return "Catalog Admin Menu"; }
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