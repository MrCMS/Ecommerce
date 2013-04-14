using MrCMS.Entities;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using System;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Orders
{
    public class OrderNote : SiteEntity
    {
        public virtual User User { get; set; }
        public virtual string Note { get; set; }
        public virtual bool ShowToClient { get; set; }
        public virtual Order Order { get; set; }
    }
}