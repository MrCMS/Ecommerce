using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.BackInStockNotification;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;
using NHibernate;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IProductUiService
    {
        bool UserNotifiedOfBackInStock(ProductVariant productVariant, bool justNotified);
        ProductVariant GetVariantToShow(Product product, int? variantId);
    }

    public class ProductUiService : IProductUiService
    {
        private readonly ISession _session;

        public ProductUiService(ISession session)
        {
            _session = session;
        }

        public bool UserNotifiedOfBackInStock(ProductVariant productVariant, bool justNotified)
        {
            if (justNotified)
                return true;
            var currentUser = CurrentRequestData.CurrentUser;
            if (currentUser == null)
                return false;
            return
                _session.QueryOver<BackInStockNotificationRequest>()
                        .Where(request => request.Email == currentUser.Email && request.ProductVariant == productVariant)
                        .Any();
        }

        public ProductVariant GetVariantToShow(Product product, int? variantId)
        {
            if (!variantId.HasValue)
                return product.Variants.FirstOrDefault();
            return product.Variants.FirstOrDefault(variant => variant.Id == variantId);
        }
    }

    public interface IProductVariantService
    {
        IList<ProductVariant> GetAll();
        IPagedList<ProductVariant> GetAllVariants(string queryTerm, int categoryId = 0, int page = 1);
        IList<ProductVariant> GetAllVariantsForGoogleBase();
        ProductVariant GetProductVariantBySKU(string sku);
        bool AnyExistingProductVariantWithSKU(string sku, int id);
        ProductVariant Get(int id);
        List<SelectListItem> GetOptions();
        PriceBreak AddPriceBreak(AddPriceBreakModel model);
        PriceBreak AddPriceBreak(PriceBreak item);
        bool IsPriceBreakQuantityValid(int quantity, ProductVariant productVariant);
        bool IsPriceBreakPriceValid(decimal price, ProductVariant productVariant, int quantity);
        void DeletePriceBreak(PriceBreak priceBreak);
        IList<ProductVariant> GetAllVariantsWithLowStock(int threshold);
        IList<ProductVariant> GetAllVariantsForStockReport();
    }
}