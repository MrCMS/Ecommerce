using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Widgets;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetRelatedProducts : GetWidgetModelBase<RelatedProducts>
    {
        private readonly IRelatedProductsService _relatedProductsService;

        public GetRelatedProducts(IRelatedProductsService relatedProductsService)
        {
            _relatedProductsService = relatedProductsService;
        }

        public override object GetModel(RelatedProducts widget)
        {
            return _relatedProductsService.GetRelatedProducts(CurrentRequestData.CurrentPage as Product);
        }
    }
}