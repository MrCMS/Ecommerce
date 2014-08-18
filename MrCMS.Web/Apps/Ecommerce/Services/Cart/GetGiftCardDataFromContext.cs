using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;
using Newtonsoft.Json;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class GetGiftCardDataFromContext : IGetGiftCardDataFromContext
    {
        public string Get(ControllerContext controllerContext)
        {
            var defaultModelBinder = new DefaultModelBinder();

            var modelType = typeof(GiftCardInfo);
            var giftCardInfo = new GiftCardInfo();
            var metadataForType = ModelMetadataProviders.Current.GetMetadataForType(() => giftCardInfo, modelType);
            metadataForType.Model = giftCardInfo;
            var modelBindingContext = new ModelBindingContext
            {
                ModelMetadata = metadataForType,
                ValueProvider = new FormValueProvider(controllerContext)
            };
            var model = defaultModelBinder.BindModel(controllerContext, modelBindingContext) as GiftCardInfo;

            return model == null ? null : JsonConvert.SerializeObject(model);
        }
    }
}