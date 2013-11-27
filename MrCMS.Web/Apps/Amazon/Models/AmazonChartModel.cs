using System;
using System.Collections.Generic;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public class AmazonChartModel
    {
        public AmazonChartModel()
        {
            From = CurrentRequestData.Now.AddDays(-29);
            To = CurrentRequestData.Now.AddDays(1);

            Data = new Dictionary<string, decimal>();
            Labels = new List<string>();
            Title = String.Empty;
        }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public string Title { get; set; }
        public bool ShowTitle
        {
            get { return !String.IsNullOrWhiteSpace(Title); }
        }

        public Dictionary<string, decimal> Data { get; set; }
        public List<string> Labels { get; set; }
    }
}