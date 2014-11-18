using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class RewardPointACL : ACLRule
    {
        public const string Settings = "Settings";

        public override string DisplayName
        {
            get { return "Reward Point"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { Settings };
        }
    }
}