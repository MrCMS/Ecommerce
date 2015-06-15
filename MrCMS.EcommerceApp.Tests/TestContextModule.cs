using System.Web;
using MrCMS.Website;
using Ninject.Modules;

namespace MrCMS.EcommerceApp.Tests
{
    public class TestContextModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Rebind<HttpContextBase>().To<OutOfContext>().InThreadScope();
        }
    }
}