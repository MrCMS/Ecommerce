using System.Web;
using Elmah.ContentSyndication;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.TemplateData;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Web.Apps.NewsletterBuilder.Entities;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;
using MrCMS.Web.Apps.NewsletterBuilder.Services;
using WebGrease.Css.Extensions;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public class SetupNewsletterTemplate : ISetupNewsletterTemplate
    {
        private readonly INewsletterTemplateAdminService _newsletterTemplateAdminService;
        private readonly ITemplateDataAdminService _templateDataAdminService;
        private readonly IGetContentItemTemplateData _getContentItemTemplateData;

        public SetupNewsletterTemplate(INewsletterTemplateAdminService newsletterTemplateAdminService, ITemplateDataAdminService templateDataAdminService, IGetContentItemTemplateData getContentItemTemplateData)
        {
            _newsletterTemplateAdminService = newsletterTemplateAdminService;
            _templateDataAdminService = templateDataAdminService;
            _getContentItemTemplateData = getContentItemTemplateData;
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
            };
            _newsletterTemplateAdminService.Add(template);


            var productListTemplate = _getContentItemTemplateData.Get<ProductListTemplateData>(template);
            productListTemplate.ProductGridTemplate = System.IO.File.ReadAllText(
                HttpContext.Current.Server.MapPath(
                    @"\Apps\Ecommerce\Installation\Content\NewsletterProductGridTemplate.txt"));
            productListTemplate.ProductRowTemplate = System.IO.File.ReadAllText(
                HttpContext.Current.Server.MapPath(
                    @"\Apps\Ecommerce\Installation\Content\NewsletterProductRowTemplate.txt"));
            productListTemplate.ProductTemplate = System.IO.File.ReadAllText(
                HttpContext.Current.Server.MapPath(
                    @"\Apps\Ecommerce\Installation\Content\NewsletterProductTemplate.txt"));
            _templateDataAdminService.Update(productListTemplate);


            var freeTextTemplate = _getContentItemTemplateData.Get<FreeTextTemplateData>(template);

            freeTextTemplate.FreeTextTemplate = System.IO.File.ReadAllText(
                HttpContext.Current.Server.MapPath(
                    @"\Apps\Ecommerce\Installation\Content\NewsletterFreeTextTemplate.txt"));
            _templateDataAdminService.Update(freeTextTemplate);


            var bannerTemplate = _getContentItemTemplateData.Get<BannerTemplateData>(template);
            bannerTemplate.BannerTemplate = System.IO.File.ReadAllText(
                HttpContext.Current.Server.MapPath(
                    @"\Apps\Ecommerce\Installation\Content\NewsletterBannerTemplate.txt"));
            _templateDataAdminService.Update(bannerTemplate);

            var imageLeftTextRightTemplateData = _getContentItemTemplateData.Get<ImageLeftAndTextRightTemplateData>(template);
            imageLeftTextRightTemplateData.ImageLeftAndTextRightTemplate = System.IO.File.ReadAllText(
                HttpContext.Current.Server.MapPath(
                    @"\Apps\Ecommerce\Installation\Content\ImageLeftAndTextRightTemplate.txt"));
            _templateDataAdminService.Update(imageLeftTextRightTemplateData);

            var imageRightTextLeftTemplateData = _getContentItemTemplateData.Get<ImageRightAndTextLeftTemplateData>(template);
            imageRightTextLeftTemplateData.ImageRightAndTextLeftTemplate = System.IO.File.ReadAllText(
                HttpContext.Current.Server.MapPath(
                    @"\Apps\Ecommerce\Installation\Content\ImageRightAndTextLeftTemplate.txt"));
            _templateDataAdminService.Update(imageRightTextLeftTemplateData);
        }
    }
}