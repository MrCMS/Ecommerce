using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Services.Notifications;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class BrandAdminService : IBrandAdminService
    {
        private readonly IUniquePageService _uniquePageService;
        private readonly ISession _session;
        private readonly IGetNewBrandPage _getNewBrandPage;

        public BrandAdminService(IUniquePageService uniquePageService, ISession session, IGetNewBrandPage getNewBrandPage)
        {
            _uniquePageService = uniquePageService;
            _session = session;
            _getNewBrandPage = getNewBrandPage;
        }

        public IPagedList<BrandPage> Search(BrandSearchModel searchModel)
        {
            var queryOver = _session.QueryOver<BrandPage>();
            if (!string.IsNullOrWhiteSpace(searchModel.Query))
            {
                queryOver = queryOver.Where(x => x.Name.IsInsensitiveLike(searchModel.Query, MatchMode.Anywhere));
            }

            return queryOver.OrderBy(page => page.CreatedOn).Asc.Paged(searchModel.Page);
        }

        public BrandListing GetListingPage()
        {
            return _uniquePageService.GetUniquePage<BrandListing>();
        }

        public bool AnyToMigrate()
        {
            return _session.QueryOver<Brand>().Where(x => !x.IsMigrated).Any();
        }

        public void MigrateBrands()
        {
            using (new NotificationDisabler())
            {
                var toMigrate = _session.QueryOver<Brand>().Where(x => !x.IsMigrated).List();

                var map = new Dictionary<Brand, BrandPage>();
                _session.Transact(session =>
                {
                    foreach (var brand in toMigrate)
                    {
                        var brandPage = _getNewBrandPage.Get(brand.Name, brand.Logo);
                        session.Save(brandPage);
                        brand.IsMigrated = true;
                        session.Update(brand);

                        map.Add(brand, brandPage);
                    }
                });

                var products = _session.QueryOver<Product>().Where(x => x.Brand != null).List();
                if (products.Any())
                {
                    _session.Transact(session =>
                    {
                        foreach (var product in products)
                        {
                            var brandPage = map[product.Brand];
                            product.BrandPage = brandPage;
                            brandPage.Products.Add(product);
                            session.Update(product);
                            session.Update(brandPage);
                        }
                    });
                }
            }
        }

        public List<SelectListItem> GetOptions()
        {
            BrandInfo info = null;
            return _session.QueryOver<BrandPage>()
                .OrderBy(x => x.Name).Asc
                .SelectList(builder =>
                {
                    builder.Select(x => x.Id).WithAlias(() => info.Id);
                    builder.Select(x => x.Name).WithAlias(() => info.Name);
                    return builder;
                }).TransformUsing(Transformers.AliasToBean<BrandInfo>())
                .List<BrandInfo>()
                .BuildSelectItemList(item => item.Name, item => item.Id.ToString(), null, new SelectListItem
                {
                    Text = "Please select...",
                    Value = "0"
                });
        }

        public class BrandInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }

    public interface IGetNewBrandPage
    {
        BrandPage Get(string name, string logo = null);
    }

    public class GetNewBrandPage : IGetNewBrandPage
    {
        private readonly IUniquePageService _uniquePageService;
        private readonly IWebpageUrlService _webpageUrlService;

        public GetNewBrandPage(IUniquePageService uniquePageService, IWebpageUrlService webpageUrlService)
        {
            _uniquePageService = uniquePageService;
            _webpageUrlService = webpageUrlService;
        }

        public BrandPage Get(string name, string logo = null)
        {

            var listing = _uniquePageService.GetUniquePage<BrandListing>();
            return new BrandPage
            {
                Name = name,
                UrlSegment =
                    _webpageUrlService.Suggest(listing,
                        new SuggestParams
                        {
                            DocumentType = typeof(BrandPage).FullName,
                            PageName = name,
                            UseHierarchy = true
                        }),
                FeatureImage = logo,
                PublishOn = CurrentRequestData.Now,
                Published = true,
                Parent = listing,
                RevealInNavigation = false
            };
        }
    }
}