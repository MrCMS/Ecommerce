using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class ProductVariantAdminViewDataService : IProductVariantAdminViewDataService
    {
        private readonly IGetGiftCardTypeOptions _getGiftCardTypeOptions;
        private readonly IGetProductVariantTypeOptions _getProductVariantTypeOptions;
        private readonly IGetTaxRateOptions _getTaxRateOptions;
        private readonly IGetTrackingPolicyOptions _getTrackingPolicyOptions;
        private readonly IGetShippingOptions _getShippingOptions;
        private readonly IProductReviewUIService _productReviewUIService;
        private readonly IETagAdminService _eTagAdminService;

        public ProductVariantAdminViewDataService(IGetGiftCardTypeOptions getGiftCardTypeOptions,
            IGetProductVariantTypeOptions getProductVariantTypeOptions, IGetTaxRateOptions getTaxRateOptions,
            IGetTrackingPolicyOptions getTrackingPolicyOptions, IGetShippingOptions getShippingOptions, IProductReviewUIService productReviewUIService, IETagAdminService eTagAdminService)
        {
            _getGiftCardTypeOptions = getGiftCardTypeOptions;
            _getProductVariantTypeOptions = getProductVariantTypeOptions;
            _getTaxRateOptions = getTaxRateOptions;
            _getTrackingPolicyOptions = getTrackingPolicyOptions;
            _getShippingOptions = getShippingOptions;
            _productReviewUIService = productReviewUIService;
            _eTagAdminService = eTagAdminService;
        }

        public void SetViewData(ViewDataDictionary viewData, ProductVariant productVariant)
        {
            viewData["gift-card-type-options"] = _getGiftCardTypeOptions.Get();
            viewData["product-variant-type-options"] = _getProductVariantTypeOptions.Get();
            viewData["tax-rate-options"] = _getTaxRateOptions.GetOptions();
            viewData["tracking-policy"] = _getTrackingPolicyOptions.Get();
            viewData["shipping-options"] = _getShippingOptions.Get(productVariant);
            viewData["e-tag-options"] = _eTagAdminService.GetOptions();
        }
    }
}