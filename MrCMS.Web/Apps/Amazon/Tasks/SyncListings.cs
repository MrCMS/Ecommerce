using MrCMS.Entities.Multisite;
using MrCMS.Tasks;

namespace MrCMS.Web.Apps.Amazon.Tasks
{
    public class SyncListings : BackgroundTask
    {
        public SyncListings(Site site) : base(site)
        {
            
        }

        public override void Execute()
        {

        }
    }
}
