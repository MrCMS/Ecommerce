using System.Drawing;

namespace MrCMS.Web.Apps.NewsletterBuilder.Services
{
    public interface INewsletterBuilderHelperService
    {
        string GetResizedImage(string imageUrl, Size size);
    }
}