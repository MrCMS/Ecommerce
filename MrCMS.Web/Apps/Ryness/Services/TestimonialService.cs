using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ryness.Entities;
using NHibernate;

namespace MrCMS.Web.Apps.Ryness.Services
{
    public class TestimonialService : ITestimonialService
    {
        private readonly ISession _session;

        public TestimonialService(ISession session)
        {
            _session = session;
        }

        public IList<Testimonial> GetAll()
        {
            return _session.QueryOver<Testimonial>().Cacheable().List();
        }

        public void Add(Testimonial testimonial)
        {
            _session.Transact(session => session.Save(testimonial));
        }

        public void Update(Testimonial testimonial)
        {
            _session.Transact(session => session.Update(testimonial));
        }

        public void Delete(Testimonial testimonial)
        {
            _session.Transact(session => session.Delete(testimonial));
        }
    }
}