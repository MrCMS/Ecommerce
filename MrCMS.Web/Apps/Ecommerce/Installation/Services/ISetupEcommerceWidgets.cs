using System.Linq;
using System.Web.UI;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Widgets;
using MrCMS.Web.Apps.Ecommerce.Installation.Models;
using MrCMS.Web.Apps.Ecommerce.Widgets;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public interface ISetupEcommerceWidgets
    {
        void Setup(PageModel pageModel, MediaModel mediaModel, LayoutModel layoutModel);
    }

    public class SetupEcommerceWidgets : ISetupEcommerceWidgets
    {
        private readonly IWidgetService _widgetService;

        public SetupEcommerceWidgets(IWidgetService widgetService)
        {
            _widgetService = widgetService;
        }

        public void Setup(PageModel pageModel, MediaModel mediaModel, LayoutModel layoutModel)
        {
            var layout = layoutModel.HomeLayout;
            var beforeContent = layoutModel.EcommerceLayout.LayoutAreas.FirstOrDefault(x => x.AreaName == "Before Content");
            var teaser1Area = layout.LayoutAreas.FirstOrDefault(x => x.AreaName == "Teaser1");
            var teaser2Area = layout.LayoutAreas.FirstOrDefault(x => x.AreaName == "Teaser2");
            var teaser3Area = layout.LayoutAreas.FirstOrDefault(x => x.AreaName == "Teaser3");
            var teaser4Area = layout.LayoutAreas.FirstOrDefault(x => x.AreaName == "Teaser4");

            var slider = new Slider
            {
                Image = mediaModel.SliderImage1.FileUrl,
                Image1 = mediaModel.SliderImage2.FileUrl,
                LayoutArea = beforeContent,
                Webpage = pageModel.HomePage
            };
            _widgetService.AddWidget(slider);

            var featuredProducts = new FeaturedProducts
            {
                LayoutArea = beforeContent,
                Webpage = pageModel.HomePage,
                ListOfFeaturedProducts = "1,2,3,4"
            };
            _widgetService.AddWidget(featuredProducts);

            var featuredCategories = new FeaturedCategories
            {
                LayoutArea = beforeContent,
                Webpage = pageModel.HomePage,
                ListOfFeaturedCategories = "1,2,3,4"
            };
            _widgetService.AddWidget(featuredCategories);

            var teaser1 = new TextWidget()
            {
                LayoutArea = teaser1Area,
                Webpage = pageModel.HomePage,
                Text = @"<div class=""padding-bottom-10""><span><img src=""/content/upload/1/default/delivery.gif"" /> </span></div><h3><a href=""#"">FREE delivery on orders over &pound;50. </a></h3><p>Orders placed Monday to Friday before 2pm will generally be picked and packed for immediate despatch. Please note that orders placed over the weekend or on public holidays will be processed on the next working day.</p>"
            };
            _widgetService.AddWidget(teaser1);

            var teaser2 = new TextWidget()
            {
                LayoutArea = teaser2Area,
                Webpage = pageModel.HomePage,
                Text = @"<div class=""padding-bottom-10""><span><img src=""/content/upload/1/default/return.gif"" /> </span></div><h3><a href=""#"">7 day no question returns.</a></h3><p>We offer a 28 Day Money Back Guarantee. If for any reason you are not completely delighted with your purchase you may download a Returns Form and return it within 28 days of receipt for a full refund or exchange.</p>"
            };
            _widgetService.AddWidget(teaser2);

            var teaser3 = new TextWidget()
            {
                LayoutArea = teaser3Area,
                Webpage = pageModel.HomePage,
                Text = @"<div class=""padding-bottom-10""><span><img src=""/content/upload/1/default/location.gif"" /> </span></div><h3><a href=""#"">Store locations.</a></h3><p>Use our store locator to find a store near you as well as information like opening times, addresses, maps and a list of facilities available at every store.</p>"};
            _widgetService.AddWidget(teaser3);

            var teaser4 = new TextWidget()
            {
                LayoutArea = teaser4Area,
                Webpage = pageModel.HomePage,
                Text = @"<div class=""padding-bottom-10""><span><img src=""/content/upload/1/default/contact.gif"" /> </span></div><h3><a href=""#"">Contact us.</a></h3><p>Our customer service team is always willing to answer your proposal concerning Samsung Service. Your message will be promptly handled under the direct supervision of our executive management.</p>"
            };
            _widgetService.AddWidget(teaser4);

        }
    }
}