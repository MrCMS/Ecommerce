using System;
using MrCMS.Entities.Multisite;
using MrCMS.Tasks;
using MrCMS.Web.Apps.Amazon.Services.Api;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Tasks
{
    public class DeleteAmazonFiles : BackgroundTask
    {
        private readonly AmazonApiService _amazonApiService;

        public DeleteAmazonFiles(Site site,AmazonApiService amazonApiService)
            : base(site)
        {
            _amazonApiService = amazonApiService;
        }

        public override void Execute()
        {
            try
            {
                _amazonApiService.DeleteAmazonFilesOlderThan(new TimeSpan(30,0,0,0));
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            
        }
    }
}