using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using System.Linq;
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
            if (productVariant.Product != null)
            {
                productVariant.Product.Variants.Add(productVariant);

                for (int i = 0; i < productVariant.AttributeValues.Count; i++)
                {
                    productVariant.AttributeValues[i].ProductVariant = productVariant;
                    if (productVariant.Product.AttributeOptions.Where(x => x.Id == productVariant.AttributeValues[i].ProductAttributeOption.Id).Count() == 0)
                    {
                        productVariant.Product.AttributeOptions.Add(productVariant.AttributeValues[i].ProductAttributeOption);
                    }
                }
                _session.Transact(session => session.Update(productVariant.Product));
                _session.Transact(session => session.Save(productVariant));
            }

            _session.Evict(typeof(ProductAttributeValue));
            _session.Evict(typeof(ProductAttributeOption));
            _session.Evict(typeof(ProductVariant));
            _session.Evict(typeof(Product));
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