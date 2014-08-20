using System;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class SetVariantTypeProperties : ISetVariantTypeProperties
    {
        public void SetProperties(ProductVariant productVariant, string variantType)
        {
            var value = Enum.Parse(typeof(VariantType), variantType);
            var valueFromRequest = value is VariantType ? (VariantType)value : VariantType.Standard;
            switch (valueFromRequest)
            {
                case VariantType.Standard:
                    productVariant.IsDownloadable = false;
                    productVariant.IsGiftCard = false;
                    break;
                case VariantType.GiftCard:
                    productVariant.IsDownloadable = false;
                    productVariant.IsGiftCard = true;
                    break;
                case VariantType.Download:
                    productVariant.IsDownloadable = true;
                    productVariant.IsGiftCard = false;
                    break;
            }
        }
    }
}