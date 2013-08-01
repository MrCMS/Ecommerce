using System.Collections.Generic;
using MrCMS.Web.Apps.Ryness.Entities;

namespace MrCMS.Web.Apps.Ryness.Services
{
    public interface ITestimonialService
    {
        IList<Testimonial> GetAll();
        void Add(Testimonial testimonial);
        void Update(Testimonial testimonial);
        void Delete(Testimonial testimonial);
    }
}