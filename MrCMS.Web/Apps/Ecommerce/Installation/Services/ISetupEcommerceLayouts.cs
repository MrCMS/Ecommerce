using MrCMS.Web.Apps.Ecommerce.Installation.Models;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public interface ISetupEcommerceLayouts
    {
        LayoutModel Setup(MediaModel mediaModel);
    }
}