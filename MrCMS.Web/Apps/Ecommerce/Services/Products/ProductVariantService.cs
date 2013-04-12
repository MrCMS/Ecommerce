using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
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
                                      productVariant.AttributeValues.ForEach(
                                          value => value.ProductVariant = productVariant);
                                      session.Save(productVariant);
                                      productVariant.AttributeValues.ForEach(session.SaveOrUpdate);
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