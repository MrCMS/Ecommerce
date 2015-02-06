using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Reports
{
    public interface IChartService
    {
        void SetBarChartLabelsAndData(ref ChartModel model,
            Dictionary<string, IList<KeyValuePair<string, decimal>>> items);

        void SetLineChartData(ref ChartModel model, Dictionary<string, IList<KeyValuePair<DateTime, decimal>>> items);
        void SetLineChartLabels(ref ChartModel model);
    }
}