using System;
using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public class AmazonSyncModel
    {
        public AmazonSyncModel()
        {
            From = CurrentRequestData.Now.AddDays(-7);
            To = CurrentRequestData.Now;
            Page = 1;
            Messages = new PagedList<AmazonProgressMessageModel>(new List<AmazonProgressMessageModel>(), Page, 10);
        }

        public Guid? TaskId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public IPagedList<AmazonProgressMessageModel> Messages { get; set; }
        public int Page { get; set; }
    }
}