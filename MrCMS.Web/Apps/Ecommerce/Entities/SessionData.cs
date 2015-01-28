using System;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public class SessionData : SiteEntity
    {
        public virtual Guid UserGuid { get; set; }
        public virtual string Key { get; set; }
        public virtual string Data { get; set; }

        public virtual DateTime? ExpireOn { get; set; }
        public virtual byte[] Salt { get; set; }
    }
}