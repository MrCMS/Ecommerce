using System;
using System.Collections.Generic;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class ChartModel
    {
        public ChartModel()
        {
            From = CurrentRequestData.Now.AddDays(-6);
            To = CurrentRequestData.Now.AddDays(1);

            Labels = new List<string>();
            MultipleData = new Dictionary<string, List<decimal>>();
            Data = new Dictionary<string, decimal>();

            Title = String.Empty;
        }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public string Title { get; set; }
        public bool ShowTitle
        {
            get { return !String.IsNullOrWhiteSpace(Title); }
        }
        
        public List<string> Labels { get; set; }
        public Dictionary<string, List<decimal>> MultipleData { get; set; }
        public Dictionary<string, decimal> Data { get; set; }
    }
}