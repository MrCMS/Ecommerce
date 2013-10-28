using System;
using System.Net;
using MrCMS.Entities.Multisite;
using MrCMS.Tasks;

namespace MrCMS.Web.Apps.Amazon.Tasks
{
    public class SyncAmazonOrderItems : BackgroundTask
    {
        public SyncAmazonOrderItems(Site site)
            : base(site)
        {
        }

        public override void Execute()
        {
            var webClient = new WebClient();

            webClient.DownloadData(new Uri(new Uri("http://" + Site.BaseUrl), "sync-amazon-order-items"));
        }
    }
}