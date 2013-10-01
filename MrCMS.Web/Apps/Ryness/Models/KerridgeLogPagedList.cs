using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ryness.Entities;

namespace MrCMS.Web.Apps.Ryness.Models
{
    public class KerridgeLogPagedList : StaticPagedList<KerridgeLog>
    {
        public KerridgeLogPagedList(IPagedList<KerridgeLog> logs)
            : base(logs, logs)
        {
        }
    }
}

