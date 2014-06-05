using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Ecommerce.Pages
{
    public class ContactUs : Webpage, IUniquePage
    {
        public virtual string Address { get; set; }
        public virtual decimal Latitude { get; set; }
        public virtual decimal Longitude { get; set; }
        public virtual string PinImage { get; set; }
    }
}