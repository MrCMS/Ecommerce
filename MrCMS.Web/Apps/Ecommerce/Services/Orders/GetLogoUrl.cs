using System;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class GetLogoUrl : IGetLogoUrl
    {
        private readonly EcommerceSettings _ecommerceSettings;

        public GetLogoUrl(EcommerceSettings ecommerceSettings)
        {
            _ecommerceSettings = ecommerceSettings;
        }

        public string Get()
        {
            string logoUrl;
            try
            {
                logoUrl = CurrentRequestData.CurrentContext.Server.MapPath(_ecommerceSettings.ReportLogoImage);
            }
            catch (Exception ex)
            {
                logoUrl = string.Empty;
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return logoUrl;
        }
    }
}