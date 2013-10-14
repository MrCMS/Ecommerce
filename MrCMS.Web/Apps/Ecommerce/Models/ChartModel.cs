using System;
using System.Collections.Generic;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class ChartModel
    {
        public ChartModel()
        {
            SalesChannel = "Mr CMS";

            From = CurrentRequestData.Now.AddMonths(-1);
            To = CurrentRequestData.Now;

            LineChartLabels = new List<string>();
            LineChartData = new Dictionary<string, List<decimal>>();
        }

        public string SalesChannel { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public List<string> LineChartLabels { get; set; }
        public Dictionary<string, List<decimal>> LineChartData { get; set; }

        public Dictionary<string, decimal> PieChartData { get; set; }
    }
}