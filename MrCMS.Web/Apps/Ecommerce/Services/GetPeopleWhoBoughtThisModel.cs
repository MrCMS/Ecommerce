using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Widgets;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetPeopleWhoBoughtThisModel : GetWidgetModelBase<PeopleWhoBoughtThisAlsoBought>
    {
        private readonly IPeopleWhoBoughtThisService _peopleWhoBoughtThisService;

        public GetPeopleWhoBoughtThisModel(IPeopleWhoBoughtThisService peopleWhoBoughtThisService)
        {
            _peopleWhoBoughtThisService = peopleWhoBoughtThisService;
        }

        public override object GetModel(PeopleWhoBoughtThisAlsoBought widget)
        {
            return _peopleWhoBoughtThisService.GetAlsoBoughtViewModel(CurrentRequestData.CurrentPage as Product);
        }
    }
}