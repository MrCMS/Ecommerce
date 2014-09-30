using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Settings;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetProductCardModel : IGetProductCardModel
    {
        private readonly ISession _session;
        private readonly IProductVariantAvailabilityService _productVariantAvailabilityService;
        private readonly EcommerceSettings _settings;

        public GetProductCardModel(ISession session, IProductVariantAvailabilityService productVariantAvailabilityService, EcommerceSettings settings)
        {
            _session = session;
            _productVariantAvailabilityService = productVariantAvailabilityService;
            _settings = settings;
        }

        public ProductCardModel Get(Product product)
        {
            return Get(new List<Product> { product }).First();
        }

        public List<ProductCardModel> Get(List<Product> products)
        {
            var galleryIds = products.Select(product => product.Gallery.Id).ToList();
            var productIds = products.Select(product => product.Id).ToList();
            List<MediaFile> mediaFiles = _session.QueryOver<MediaFile>()
               .Where(file => file.MediaCategory.Id.IsIn(galleryIds))
               .OrderBy(file => file.DisplayOrder)
               .Asc.Cacheable()
               .List().ToList();
            List<ProductVariant> variants = _session.QueryOver<ProductVariant>()
                .Where(productVariant => productVariant.Product.Id.IsIn(productIds))
                .Cacheable()
                .List().ToList();

            var productCardModels = new List<ProductCardModel>();
            foreach (var product in products)
            {
                MediaFile image = mediaFiles.FirstOrDefault(file => file.IsImage && file.MediaCategory.Id == product.Gallery.Id);
                var productVariants = variants.FindAll(productVariant => productVariant.Product.Id == product.Id);
                ProductVariant variant = productVariants.Count == 1
                    ? productVariants.FirstOrDefault(productVariant => _productVariantAvailabilityService.CanBuy(productVariant, 1).OK)
                    : null;
                var productCardModel = new ProductCardModel
                {
                    Name = product.Name,
                    Url = product.LiveUrlSegment,
                    Abstract = product.Abstract,
                    Image = image == null ? null : image.FileUrl,
                    PreviousPriceText = _settings.PreviousPriceText
                };
                if (variant != null)
                {
                    productCardModel.PreviousPrice = product.ShowPreviousPrice ? variant.PreviousPrice : null;
                    productCardModel.Price = variant.Price;
                    productCardModel.VariantId = variant.Id;
                }
                else
                {
                    productCardModel.Price = productVariants.Any() ? productVariants.First().Price : (decimal?)null;
                }
                productCardModels.Add(productCardModel);
            }
            return productCardModels;
        }
    }
}