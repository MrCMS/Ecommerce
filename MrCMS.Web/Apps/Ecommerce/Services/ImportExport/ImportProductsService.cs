using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Batching.CoreJobs;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Indexing;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Batching;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductsService : IImportProductsService
    {
        private readonly ICreateBatchRun _createBatchRun;
        private readonly IDocumentService _documentService;
        private readonly IImportProductImagesService _importProductImagesService;
        private readonly IImportProductVariantsService _importProductVariantsService;
        private readonly IImportProductSpecificationsService _importSpecificationsService;
        private readonly IImportProductUrlHistoryService _importUrlHistoryService;
        private readonly ISession _session;
        private readonly IUniquePageService _uniquePageService;


        public ImportProductsService(IDocumentService documentService,
            IImportProductSpecificationsService importSpecificationsService,
            IImportProductVariantsService importProductVariantsService,
            IImportProductImagesService importProductImagesService,
            IImportProductUrlHistoryService importUrlHistoryService, ISession session,
            IUniquePageService uniquePageService, ICreateBatchRun createBatchRun)
        {
            _documentService = documentService;
            _importSpecificationsService = importSpecificationsService;
            _importProductVariantsService = importProductVariantsService;
            _importProductImagesService = importProductImagesService;
            _importUrlHistoryService = importUrlHistoryService;
            _session = session;
            _uniquePageService = uniquePageService;
            _createBatchRun = createBatchRun;
        }

        public void CreateBatch(HashSet<ProductImportDataTransferObject> productsToImport)
        {
            var batch = new Batch { BatchJobs = new List<BatchJob>() };
            _session.Transact(session => session.Save(batch));
            _session.Transact(session =>
            {
                foreach (ProductImportDataTransferObject item in productsToImport)
                {
                    var importDocumentBatchJob = new ImportProductBatchJob
                    {
                        Batch = batch,
                        Data = JsonConvert.SerializeObject(item),
                        ProductName = item.Name,
                        UrlSegment = item.UrlSegment
                    };
                    batch.BatchJobs.Add(importDocumentBatchJob);
                    session.Save(importDocumentBatchJob);
                }
                // Reindex Universal search when done
                var universalIndexRebuilder = new RebuildUniversalSearchIndex
                {
                    Batch = batch
                };
                batch.BatchJobs.Add(universalIndexRebuilder);
                session.Save(universalIndexRebuilder);

                // Reindex standard indexes
                foreach (var type in IndexingHelper.IndexDefinitionTypes)
                {
                    var luceneIndex = new RebuildLuceneIndex
                    {
                        Batch = batch,
                        IndexName = type.SystemName
                    };
                    batch.BatchJobs.Add(luceneIndex);
                    session.Save(luceneIndex);
                }
            });
            _createBatchRun.Create(batch);
        }

        /// <summary>
        ///     Import from DTOs
        /// </summary>
        /// <param name="dataTransferObject"></param>
        public Product ImportProduct(ProductImportDataTransferObject dataTransferObject)
        {
            var uniquePage = _uniquePageService.GetUniquePage<ProductContainer>();
            var productGalleriesCategory = _documentService.GetDocumentByUrl<MediaCategory>("product-galleries");
            if (productGalleriesCategory == null)
            {
                productGalleriesCategory = new MediaCategory
                {
                    Name = "Product Galleries",
                    UrlSegment = "product-galleries",
                    IsGallery = true,
                    HideInAdminNav = true
                };
                _documentService.AddDocument(productGalleriesCategory);
            }


            Product product =
                _session.Query<Product>()
                    .SingleOrDefault(x => x.UrlSegment == dataTransferObject.UrlSegment) ??
                new Product();

            product.Parent = uniquePage;
            product.UrlSegment = dataTransferObject.UrlSegment;
            product.Name = dataTransferObject.Name;
            product.BodyContent = dataTransferObject.Description;
            product.MetaTitle = dataTransferObject.SEOTitle;
            product.MetaDescription = dataTransferObject.SEODescription;
            product.MetaKeywords = dataTransferObject.SEOKeywords;
            product.Abstract = dataTransferObject.Abstract;
            product.PublishOn = dataTransferObject.PublishDate;

            bool isNew = false;
            MediaCategory productGallery = product.Gallery ?? new MediaCategory();
            if (product.Id == 0)
            {
                isNew = true;
                product.DisplayOrder =
                    GetParentQuery(uniquePage).Any()
                        ? GetParentQuery(uniquePage)
                            .Select(Projections.Max<Webpage>(webpage => webpage.DisplayOrder))
                            .SingleOrDefault<int>()
                        : 0;
                productGallery.Name = product.Name;
                productGallery.UrlSegment = "product-galleries/" + product.UrlSegment;
                productGallery.IsGallery = true;
                productGallery.Parent = productGalleriesCategory;
                productGallery.HideInAdminNav = true;
                product.Gallery = productGallery;
            }

            SetBrand(dataTransferObject, product);

            SetCategories(dataTransferObject, product);

            SetOptions(dataTransferObject, product);

            ////Url History
            _importUrlHistoryService.ImportUrlHistory(dataTransferObject, product);

            ////Specifications
            _importSpecificationsService.ImportSpecifications(dataTransferObject, product);

            ////Variants
            _importProductVariantsService.ImportVariants(dataTransferObject, product);

            if (isNew)
            {
                _session.Transact(session => session.Save(product));
                _session.Transact(session => session.Save(productGallery));
            }
            else
            {
                _session.Transact(session => session.Update(product));
                _session.Transact(session => session.Update(productGallery));
            }

            _importProductImagesService.ImportProductImages(dataTransferObject.Images, product.Gallery);

            return product;
        }

        private IQueryOver<Webpage, Webpage> GetParentQuery(ProductContainer uniquePage)
        {
            return _session.QueryOver<Webpage>().Where(webpage => webpage.Parent.Id == uniquePage.Id);
        }

        private void SetBrand(ProductImportDataTransferObject dataTransferObject, Product product)
        {
            //Brand
            if (!String.IsNullOrWhiteSpace(dataTransferObject.Brand))
            {
                string dtoBrand = dataTransferObject.Brand.Trim();
                Brand brand =
                    _session.QueryOver<Brand>()
                        .Where(b => b.Name.IsInsensitiveLike(dtoBrand, MatchMode.Exact))
                        .Take(1)
                        .SingleOrDefault();
                if (brand == null)
                {
                    brand = new Brand { Name = dtoBrand };
                    _session.Transact(session => session.Save(brand));
                }
                product.Brand = brand;
            }
        }

        private void SetOptions(ProductImportDataTransferObject dataTransferObject, Product product)
        {
            List<string> optionsToAdd =
                dataTransferObject.Options.Where(
                    s => !product.Options.Select(option => option.Name).Contains(s, StringComparer.OrdinalIgnoreCase))
                    .ToList();
            List<ProductOption> optionsToRemove =
                product.Options.Where(option => !dataTransferObject.Options.Contains(option.Name)).ToList();

            foreach (string option in optionsToAdd)
            {
                ProductOption existingOption =
                    _session.QueryOver<ProductOption>()
                        .Where(productOption => productOption.Name.IsInsensitiveLike(option, MatchMode.Exact))
                        .Take(1).SingleOrDefault();
                if (existingOption == null)
                {
                    existingOption = new ProductOption
                    {
                        Name = option,
                    };

                    _session.Transact(session => session.Save(existingOption));
                }
                product.Options.Add(existingOption);
                existingOption.Products.Add(product);
            }
            foreach (ProductOption option in optionsToRemove)
            {
                product.Options.Remove(option);
                option.Products.Remove(product);
            }
        }

        public void SetCategories(ProductImportDataTransferObject dataTransferObject, Product product)
        {
            //Categories
            List<string> categoriesToAdd =
                dataTransferObject.Categories.Where(
                    s =>
                        !product.Categories.Select(category => category.UrlSegment)
                            .Contains(s, StringComparer.OrdinalIgnoreCase)).ToList();
            List<Category> categoriesToRemove =
                product.Categories.Where(
                    category =>
                        !dataTransferObject.Categories.Contains(category.UrlSegment, StringComparer.OrdinalIgnoreCase))
                    .ToList();
            foreach (string item in categoriesToAdd)
            {
                Category category =
                    _session.QueryOver<Category>()
                        .Where(c => c.UrlSegment.IsInsensitiveLike(item, MatchMode.Exact))
                        .Take(1)
                        .SingleOrDefault();
                if (category != null)
                {
                    product.Categories.Add(category);
                    if (!category.Products.Contains(product))
                        category.Products.Add(product);
                }
            }
            foreach (Category category in categoriesToRemove)
            {
                product.Categories.Remove(category);
                if (category.Products.Contains(product))
                    category.Products.Remove(product);
            }
        }
    }
}