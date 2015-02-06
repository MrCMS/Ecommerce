using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class EcommerceSearchablePageAdminService : IEcommerceSearchablePageAdminService
    {
        private readonly ISession _session;

        public EcommerceSearchablePageAdminService(ISession session)
        {
            _session = session;
        }

        public List<ProductSpecificationAttribute> GetShownSpecifications(EcommerceSearchablePage category)
        {
            return _session.QueryOver<ProductSpecificationAttribute>()
                .Cacheable()
                .List()
                .Where(attribute => !category.HiddenSearchSpecifications.Contains(attribute))
                .ToList();
        }

        public bool AddSpecificationToHidden(ProductSpecificationAttribute attribute, int categoryId)
        {
            var category = _session.Get<EcommerceSearchablePage>(categoryId);

            if (category == null)
                return false;
            category.HiddenSearchSpecifications.Add(attribute);
            attribute.HiddenInSearchpages.Add(category);
            _session.Transact(session => session.Update(category));
            return true;
        }

        public bool RemoveSpecificationFromHidden(ProductSpecificationAttribute attribute, int categoryId)
        {
            var category = _session.Get<EcommerceSearchablePage>(categoryId);

            if (category == null)
                return false;
            category.HiddenSearchSpecifications.Remove(attribute);
            attribute.HiddenInSearchpages.Remove(category);
            _session.Transact(session => session.Update(category));
            return true;
        }
    }
}