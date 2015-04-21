using System;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Paypoint
{
    public class PaypointResponse : SiteEntity
    {
        public virtual string RawData { get; set; }
        public virtual string Response { get; set; }
    }
}