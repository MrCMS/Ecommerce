using System;
using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public class AmazonSyncModel
    {
        public AmazonSyncModel()
        {
            Page = 1;
            From = CurrentRequestData.Now.AddDays(-7);
            To = CurrentRequestData.Now;
            Messages = new PagedList<AmazonProgressMessageModel>(new List<AmazonProgressMessageModel>(), Page, 10);
            Id = 0;
            Description = String.Empty;
        }

        public Guid Task { get
        {
            if (!TaskId.HasValue)
                TaskId = Guid.NewGuid();
            return TaskId.Value;
        }}
        public Guid? TaskId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public IPagedList<AmazonProgressMessageModel> Messages { get; set; }
        public int Page { get; set; }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public AmazonListingGroup AmazonListingGroup { get; set; }
        public AmazonListing AmazonListing { get; set; }
    }
}