using MrCMS.Web.Apps.Ecommerce.Entities;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IProductVariantService
    {
        void Add(ProductVariant productVariant);
        void Update(ProductVariant productVariant);
        void Delete(ProductVariant productVariant);
    }

    public class ProductVariantService : IProductVariantService
    {
        private readonly ISession _session;

        public ProductVariantService(ISession session)
        {
            _session = session;
        }

        public void Add(ProductVariant productVariant)
        {
            _session.Transact(session =>
                                  {
                                      if (productVariant.Product != null)
                                          productVariant.Product.Variants.Add(productVariant);
                                      session.Save(productVariant);
                                  });
        }

        public void Update(ProductVariant productVariant)
        {
            _session.Transact(session => session.Update(productVariant));
        }

        public void Delete(ProductVariant productVariant)
        {
            _session.Transact(session => session.Delete(productVariant));
        }
    }
}