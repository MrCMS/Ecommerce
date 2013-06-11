using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using NHibernate;
using NHibernate.Criterion;
using System.Collections.Generic;
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
        public ProductVariant GetProductVariantBySKU(string sku)
        {
            return _session.QueryOver<ProductVariant>()
                            .Where(
                                variant =>
                                variant.SKU.IsInsensitiveLike(sku, MatchMode.Exact)).SingleOrDefault();
        }
        public void Add(ProductVariant productVariant)
        {
            if (productVariant.Product != null)
            {
                if (productVariant.Weight != 0)
                    productVariant.Weight = productVariant.Product.Weight;
                productVariant.Product.Variants.Add(productVariant);

                foreach (ProductAttributeValue t in productVariant.AttributeValues)
                {
                    t.ProductVariant = productVariant;
                    if (productVariant.Product.AttributeOptions.All(x => x.Id != t.ProductAttributeOption.Id))
                    {
                        productVariant.Product.AttributeOptions.Add(t.ProductAttributeOption);
                    }
                }
                _session.Transact(session => session.Save(productVariant));
            }
        }

        public void Update(ProductVariant productVariant)
        {
            _session.Transact(session => session.Update(productVariant));
        }

        public void Delete(ProductVariant productVariant)
        {
            _session.Transact(session => session.Delete(productVariant));
        }

        public bool AnyExistingProductVariantWithSKU(string sku, int id)
        {
            return _session.QueryOver<ProductVariant>()
                           .Where(
                               productVariant =>
                               productVariant.SKU.IsInsensitiveLike(sku, MatchMode.Exact) && productVariant.Id != id)
                           .RowCount() > 0;
        }
        public IList<PriceBreak> GetAllPriceBreaksForProductVariant(int id)
        {
            if (id != 0)
            {
                ProductVariant productVariant = _session.QueryOver<ProductVariant>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
                if (productVariant.PriceBreaks.Count() > 0)
                    return productVariant.PriceBreaks;
                else
                    return new List<PriceBreak>();
            }
            else
            {
                return new List<PriceBreak>();
            }
        }
    }
}