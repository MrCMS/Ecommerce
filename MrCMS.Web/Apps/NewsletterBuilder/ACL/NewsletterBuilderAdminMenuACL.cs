using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.NewsletterBuilder.ACL
{
    public class NewsletterBuilderAdminMenuACL : ACLRule
    {
        public const string ShowMenu = "Show Menu";

        public override string DisplayName
            => "Newsletter Builder Admin Menu";

        protected override List<string> GetOperations() 
            => new List<string>
                {
                    ShowMenu
                };
    }
}
