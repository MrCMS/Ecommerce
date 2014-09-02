using System.Web.UI;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Ecommerce.Installation.Models;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public interface ISetupEcommerceWidgets
    {
        void Setup(PageModel pageModel, MediaModel mediaModel, LayoutModel layoutModel);
    }
}