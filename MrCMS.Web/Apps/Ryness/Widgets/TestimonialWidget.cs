using System;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Ryness.Entities;

namespace MrCMS.Web.Apps.Ryness.Widgets
{
    public class TestimonialWidget : Widget
    {
        public override object GetModel(NHibernate.ISession session)
        {
            var query = session.QueryOver<Testimonial>().Cacheable().List();
            var count = query.Count;
            if (count > 0)
            {
                var index = new Random().Next(count);
                Testimonial testimonial = query[index];
                return testimonial;
            }
            return null;
        }
    }
}