using System.Collections.Generic;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public class AmazonChartModel
    {
        public AmazonChartModel()
        {
            Data=new List<decimal>();
            Labels=new List<string>();
        }

        public List<decimal> Data { get; set; }
        public List<string> Labels { get; set; }
    }
}