using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Models.StockAvailability;
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
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly EcommerceSettings _settings;

        public GetProductCardModel(ISession session, IProductVariantAvailabilityService productVariantAvailabilityService, IStringResourceProvider stringResourceProvider, EcommerceSettings settings)
        {
            _session = session;
            _productVariantAvailabilityService = productVariantAvailabilityService;
            _stringResourceProvider = stringResourceProvider;
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
                var productCardModel = new ProductCardModel
                {
                    Name = product.Name,
                    Url = product.LiveUrlSegment,
                    Abstract = product.ProductAbstract,
                    Image = image == null ? null : image.FileUrl,
                    PreviousPriceText = _settings.PreviousPriceText,
                    IsMultiVariant = productVariants.Count > 1
                };
                if (productVariants.Count == 1)
                {
                    var variant = productVariants.FirstOrDefault();
                    productCardModel.PreviousPrice = product.ShowPreviousPrice ? variant.PreviousPrice : null;
                    productCardModel.Price = variant.Price;
                    productCardModel.VariantId = variant.Id;
                    CanBuyStatus canBuyStatus = _productVariantAvailabilityService.CanBuy(variant, 1);
                    productCardModel.CanBuyStatus = canBuyStatus;
                    productCardModel.StockMessage = canBuyStatus.OK
                        ? (!string.IsNullOrEmpty(variant.CustomStockInStockMessage)
                            ? variant.CustomStockInStockMessage
                            : _stringResourceProvider.GetValue("In Stock"))
                        : (!string.IsNullOrEmpty(variant.CustomStockOutOfStockMessage)
                            ? variant.CustomStockOutOfStockMessage
                            : _stringResourceProvider.GetValue("Out of Stock"));
                }
                else
                {
                    productCardModel.Price = productVariants.Any() ? productVariants.Min(x => x.Price) : (decimal?)null;
                }
                productCardModels.Add(productCardModel);
            }
            return productCardModels;
        }

        public List<ProductCardModel> Get(List<int> productIds)
        {
            var products = _session.QueryOver<Product>().Where(product => product.Id.IsIn(productIds)).Cacheable().List();

            return Get(products.OrderBy(product => productIds.IndexOf(product.Id)).ToList());
        }
    }
}