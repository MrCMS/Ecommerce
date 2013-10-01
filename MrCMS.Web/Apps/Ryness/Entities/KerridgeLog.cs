using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ryness.Entities
{
    public class KerridgeLog : SiteEntity
    {
        public virtual Order Order { get; set; }
        public virtual bool Sent { get; set; }
        public virtual string Message { get; set; }
    }
}