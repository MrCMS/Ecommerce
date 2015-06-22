using System;
using System.Collections.Generic;
using System.Linq;
using Iesi.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;
using NHibernate;
using Brand = MrCMS.Web.Apps.Ecommerce.Pages.Brand;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportProducts : IImportProducts
    {
        private readonly IImportProductVariants _importProductVariants;
        private readonly IStatelessSession _session;
        private readonly Site _site;
        private readonly IUniquePageService _uniquePageService;
        private readonly IWebpageUrlService _webpageUrlService;

        public ImportProducts(IUniquePageService uniquePageService, IStatelessSession session, Site site,
            IWebpageUrlService webpageUrlService, IImportProductVariants importProductVariants)
        {
            _uniquePageService = uniquePageService;
            _session = session;
            _site = site;
            _webpageUrlService = webpageUrlService;
            _importProductVariants = importProductVariants;
        }

        public string ProcessProducts(NopCommerceDataReader dataReader,
            NopImportContext nopImportContext)
        {
            HashSet<ProductData> productDatas = dataReader.GetProducts();
            HashSet<ProductOptionValueData> optionValues = dataReader.GetProductOptionValues();
            HashSet<ProductSpecificationValueData> specificationValues = dataReader.GetProductSpecificationValues();

            var productContainer =
                _session.Get<ProductContainer>(_uniquePageService.GetUniquePage<ProductContainer>().Id);
            var site = _session.Get<Site>(_site.Id);

            var topLevelGallery = GetTopLevelGallery();

            _session.Transact(session =>
            {
                foreach (ProductData productData in productDatas)
                {
                    var suggestParams = new SuggestParams
                    {
                        DocumentType = typeof(Product).FullName,
                        PageName = productData.Name,
                        UseHierarchy = true
                    };
                    var product = new Product
                    {
                        Name = productData.Name,
                        ProductAbstract = productData.Abstract.LimitCharacters(500),
                        BodyContent = productData.Description,
                        Parent = productContainer,
                        UrlSegment =
                            string.IsNullOrWhiteSpace(productData.Url)
                                ? _webpageUrlService.Suggest(productContainer, suggestParams)
                                : productData.Url,
                        BrandPage =
                            productData.BrandId.HasValue
                                ? nopImportContext.FindNew<Brand>(productData.BrandId.Value)
                                : null,
                        Categories = productData.Categories.Select(nopImportContext.FindNew<Category>).ToList(),
                        Tags = new HashSet<Tag>(productData.Tags.Select(nopImportContext.FindNew<Tag>).ToList()),
                        PublishOn = productData.Published ? CurrentRequestData.Now.Date : (DateTime?)null
                    };
                    product.AssignBaseProperties(site);

                    var productGallery = GetProductGallery(product, topLevelGallery);
                    productGallery.AssignBaseProperties(site);
                    session.Insert(productGallery);

                    product.Gallery = productGallery;
                    session.Insert(product);

                    var productSpecificationValues = SetSpecificationValues(nopImportContext,
                        specificationValues.FindAll(data => data.ProductId == productData.Id), product, site);
                    foreach (var value in productSpecificationValues)
                        session.Insert(value);

                    var variants = _importProductVariants.CreateProductVariants(nopImportContext, productData.ProductVariants,
                        optionValues, product);
                    foreach (var variant in variants)
                    {
                        product.Variants.Add(variant);
                        variant.AssignBaseProperties(site);
                        session.Insert(variant);
                        session.Update(product);
                    }
                    var pictureIds = productData.Pictures;
                    foreach (var pictureId in pictureIds)
                    {
                        var mediaFile = nopImportContext.FindNew<MediaFile>(pictureId);
                        if (mediaFile != null)
                        {
                            mediaFile.MediaCategory = product.Gallery;
                            session.Update(mediaFile);
                        }
                    }
                }
            });
            return string.Format("{0} products processed.", productDatas.Count);
        }

        private MediaCategory GetTopLevelGallery()
        {
            return _session.QueryOver<MediaCategory>().Where(x => x.UrlSegment == "product-galleries").SingleOrDefault();
        }

        private MediaCategory GetProductGallery(Product product, MediaCategory mediaCategory)
        {
            return new MediaCategory
            {
                Name = product.Name,
                UrlSegment = "product-galleries/" + product.UrlSegment,
                IsGallery = true,
                Parent = mediaCategory,
                HideInAdminNav = true
            };
        }

        private static IEnumerable<ProductSpecificationValue> SetSpecificationValues(NopImportContext nopImportContext,
            HashSet<ProductSpecificationValueData> specificationValues, Product product, Site site)
        {
            foreach (
                ProductSpecificationValueData valueData in
                    specificationValues)
            {
                var specificationValue = new ProductSpecificationValue
                {
                    ProductSpecificationAttributeOption =
                        nopImportContext.FindNew<ProductSpecificationAttributeOption>(valueData.OptionId),
                    Product = product,
                    DisplayOrder = valueData.DisplayOrder
                };
                specificationValue.AssignBaseProperties(site);
                nopImportContext.AddEntry(valueData.Id, specificationValue);
                yield return specificationValue;
            }
        }
    }

    public static class ImportHelper
    {
        public static string LimitCharacters(this string value, int characters)
        {
            value = value ?? string.Empty;
            return value.Length <= characters
                ? value
                : value.Substring(0, characters);
        }
    }
}