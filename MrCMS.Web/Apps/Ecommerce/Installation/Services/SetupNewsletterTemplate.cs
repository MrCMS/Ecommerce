using System.Web;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Web.Apps.NewsletterBuilder.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public class SetupNewsletterTemplate : ISetupNewsletterTemplate
    {
        private readonly INewsletterTemplateAdminService _newsletterTemplateAdminService;

        public SetupNewsletterTemplate(INewsletterTemplateAdminService newsletterTemplateAdminService)
        {
            _newsletterTemplateAdminService = newsletterTemplateAdminService;
        }

        public void Setup()
        {

            var template = new NewsletterTemplate
            {
                Name = "Default Newsletter Template",
                BaseTemplate =
                    System.IO.File.ReadAllText(
                        HttpContext.Current.Server.MapPath(
                            @"\Apps\Ecommerce\Installation\Content\NewsletterBaseTemplate.txt")),
                Divider =
                    System.IO.File.ReadAllText(
                        HttpContext.Current.Server.MapPath(@"\Apps\Ecommerce\Installation\Content\NewsletterDivider.txt")),
                //FreeTextTemplate =
                //    System.IO.File.ReadAllText(
                //        HttpContext.Current.Server.MapPath(
                //            @"\Apps\Ecommerce\Installation\Content\NewsletterFreeTextTemplate.txt")),
                //ImageLeftAndTextRightTemplate =
                //    System.IO.File.ReadAllText(
                //        HttpContext.Current.Server.MapPath(
                //            @"\Apps\Ecommerce\Installation\Content\ImageLeftAndTextRightTemplate.txt")),
                //ImageRightAndTextLeftTemplate =
                //    System.IO.File.ReadAllText(
                //        HttpContext.Current.Server.MapPath(
                //            @"\Apps\Ecommerce\Installation\Content\ImageRightAndTextLeftTemplate.txt")),
                //ProductGridTemplate =
                //    System.IO.File.ReadAllText(
                //        HttpContext.Current.Server.MapPath(
                //            @"\Apps\Ecommerce\Installation\Content\NewsletterProductGridTemplate.txt")),
                //ProductRowTemplate =
                //    System.IO.File.ReadAllText(
                //        HttpContext.Current.Server.MapPath(
                //            @"\Apps\Ecommerce\Installation\Content\NewsletterProductRowTemplate.txt")),
                //ProductTemplate =
                //    System.IO.File.ReadAllText(
                //        HttpContext.Current.Server.MapPath(
                //            @"\Apps\Ecommerce\Installation\Content\NewsletterProductTemplate.txt")),
                //BannerTemplate =
                //    System.IO.File.ReadAllText(
                //        HttpContext.Current.Server.MapPath(
                //            @"\Apps\Ecommerce\Installation\Content\NewsletterBannerTemplate.txt")),
            };
            _newsletterTemplateAdminService.Add(template);
        }
    }
}