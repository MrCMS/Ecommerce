using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using Ninject.MockingKernel;

namespace MrCMS.EcommerceApp.Tests.Entities.Shipping
{
    public abstract class ShippingCalculationTests : MrCMSTest
    {
        private readonly TaxSettings _taxSettings;

        protected ShippingCalculationTests()
        {
            _taxSettings = new TaxSettings();
            Kernel.Bind<TaxSettings>().ToConstant(TaxSettings);
        }

        protected TaxSettings TaxSettings
        {
            get { return _taxSettings; }
        }
    }
}