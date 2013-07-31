using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Apps.Ryness.Widgets
{
    public class Slider : Widget
    {
        public virtual string SliderImage1 { get; set; }
        public virtual string SliderUrl1 { get; set; }
        public virtual string SliderImage2 { get; set; }
        public virtual string SliderUrl2 { get; set; }
        public virtual string SliderImage3 { get; set; }
        public virtual string SliderUrl3 { get; set; }
        public virtual string SliderImage4 { get; set; }
        public virtual string SliderUrl4 { get; set; }
    }
}