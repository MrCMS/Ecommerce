using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class DiscountApplicationAdminService : IDiscountApplicationAdminService
    {
        private readonly ISession _session;

        public DiscountApplicationAdminService(ISession session)
        {
            _session = session;
        }

        public List<SelectListItem> GetTypeOptions()
        {
            return TypeHelper.GetAllConcreteMappedClassesAssignableFrom<DiscountApplication>()
                .BuildSelectItemList(type => type.Name.BreakUpString(), type => type.FullName,
                    emptyItemText: "Select type...");
        }

        public IList<DiscountApplication> GetApplications(Discount discount)
        {
            return
                _session.QueryOver<DiscountApplication>().Where(application => application.Discount.Id == discount.Id)
                    .Cacheable().List();
        }

        public void Add(DiscountApplication application)
        {
            _session.Transact(session => session.Save(application));
        }

        public void Update(DiscountApplication application)
        {
            _session.Transact(session => session.Update(application));
        }

        public void Delete(DiscountApplication application)
        {
            _session.Transact(session => session.Delete(application));
        }
    }
}