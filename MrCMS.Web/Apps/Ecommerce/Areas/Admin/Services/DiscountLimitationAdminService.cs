using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class DiscountLimitationAdminService : IDiscountLimitationAdminService
    {
        private readonly ISession _session;

        public DiscountLimitationAdminService(ISession session)
        {
            _session = session;
        }

        public IList<DiscountLimitation> GetLimitations(Discount discount)
        {
            return
                _session.QueryOver<DiscountLimitation>()
                    .Where(limitation => limitation.Discount.Id == discount.Id)
                    .Cacheable()
                    .List();
        }

        public void Add(DiscountLimitation limitation)
        {
            _session.Transact(session => session.Save(limitation));
        }

        public void Update(DiscountLimitation limitation)
        {
            _session.Transact(session => session.Update(limitation));
        }

        public void Delete(DiscountLimitation limitation)
        {
            _session.Transact(session => session.Delete(limitation));
        }

        public List<SelectListItem> GetTypeOptions()
        {
            HashSet<Type> types = TypeHelper.GetAllConcreteMappedClassesAssignableFrom<DiscountLimitation>();

            return types.BuildSelectItemList(type => type.Name.BreakUpString(), type => type.FullName,
                emptyItemText: "Select type...");
        }
    }
}