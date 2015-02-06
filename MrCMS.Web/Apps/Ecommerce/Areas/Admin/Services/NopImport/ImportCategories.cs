using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport
{
    public class ImportCategories : IImportCategories
    {
        private readonly IUniquePageService _uniquePageService;
        private readonly ISession _session;
        private readonly IWebpageUrlService _webpageUrlService;

        public ImportCategories(IUniquePageService uniquePageService, ISession session, IWebpageUrlService webpageUrlService)
        {
            _uniquePageService = uniquePageService;
            _session = session;
            _webpageUrlService = webpageUrlService;
        }

        public string ProcessCategories(NopCommerceDataReader dataReader, NopImportContext nopImportContext)
        {
            var categoryDatas = dataReader.GetCategoryData();

            IEnumerable<CategoryData> parentCategories = categoryDatas.Where(data => !data.ParentId.HasValue);

            var productSearch = _uniquePageService.GetUniquePage<ProductSearch>();
            _session.Transact(session =>
            {
                foreach (CategoryData categoryData in parentCategories)
                {
                    UpdateCategory(session, categoryData, productSearch, categoryDatas, nopImportContext);
                }
            });
            return string.Format("{0} categories processed.", categoryDatas.Count);
        }

        private void UpdateCategory(ISession session, CategoryData categoryData, Webpage parent, HashSet<CategoryData> allData, NopImportContext nopImportContext)
        {
            CategoryData data = categoryData;
            var suggestParams = new SuggestParams
            {
                DocumentType = typeof(Category).FullName,
                PageName = data.Name,
                UseHierarchy = true
            };
            var category = new Category
            {
                Name = data.Name,
                UrlSegment = string.IsNullOrWhiteSpace(data.Url) ? _webpageUrlService.Suggest(parent, suggestParams) : data.Url,
                Parent = parent,
                CategoryAbstract = data.Abstract.LimitCharacters(500),
                PublishOn = data.Published ? CurrentRequestData.Now.Date : (DateTime?)null,
                RevealInNavigation = true
            };
            var mediaFile = nopImportContext.FindNew<MediaFile>(data.PictureId);
            if (mediaFile != null)
            {
                category.FeatureImage = mediaFile.FileUrl;
            }
            session.Save(category);
            nopImportContext.AddEntry(data.Id, category);
            List<CategoryData> children = allData.Where(d => d.ParentId == data.Id).ToList();
            foreach (CategoryData child in children)
            {
                UpdateCategory(session, child, category, allData, nopImportContext);
            }
        }
    }
}