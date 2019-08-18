using System;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class SalesByDayViewModel
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public List<Sales> Sales { get; set; }
    }
}