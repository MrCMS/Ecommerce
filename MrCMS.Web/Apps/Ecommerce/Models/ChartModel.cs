using System;
using System.Collections.Generic;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class ChartModel
    {
        public ChartModel()
        {
            From = CurrentRequestData.Now.AddDays(-29);
            To = CurrentRequestData.Now.AddDays(1);

            ChartLabels = new List<string>();
            MultiChartData = new Dictionary<string, List<decimal>>();
            SingleChartData = new Dictionary<string, decimal>();
        }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public List<string> ChartLabels { get; set; }
        public Dictionary<string, List<decimal>> MultiChartData { get; set; }
        public Dictionary<string, decimal> SingleChartData { get; set; }
        public Dictionary<string, decimal> PieChartData { get; set; }
    }
}