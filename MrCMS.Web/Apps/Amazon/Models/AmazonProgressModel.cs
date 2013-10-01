using System;
using System.Collections.Generic;
using MrCMS.Paging;
using System.Linq;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public class AmazonProgressModel
    {
        public AmazonProgressModel()
        {
            Messages = new List<AmazonProgressMessageModel>();
            TotalActions = 100;
            ProcessedActions = 0;
            StartTime = CurrentRequestData.Now.Date;
            TaskId = Guid.NewGuid();
        }

        public Guid TaskId { get; set; }
        public DateTime StartTime { get; set; }

        public IList<AmazonProgressMessageModel> Messages { get; set; }
        public IPagedList<AmazonProgressMessageModel> GetMessages(int page=1,int pageSize=10)
        {
            return new PagedList<AmazonProgressMessageModel>(Messages.Reverse(),page,pageSize);
        }

        public int TotalActions { get; set; }
        public int ProcessedActions { get; set; }

        public decimal PercentComplete
        {
            get { return Math.Round(((decimal) ProcessedActions/TotalActions)*100, 0); }
        }
        public bool IsComplete
        {
            get { return TotalActions == ProcessedActions; }
        }
    }
}