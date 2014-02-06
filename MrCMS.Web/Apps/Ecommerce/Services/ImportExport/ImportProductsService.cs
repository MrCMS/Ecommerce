﻿using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductsService : IImportProductsService
    {
        private readonly IDocumentService _documentService;
        private readonly IBrandService _brandService;
        private readonly IImportProductSpecificationsService _importSpecificationsService;
        private readonly IImportProductVariantsService _importProductVariantsService;
        private readonly IImportProductImagesService _importProductImagesService;
        private readonly IImportProductUrlHistoryService _importUrlHistoryService;
        private readonly ISession _session;
        private HashSet<Document> _allDocuments;
        private HashSet<Brand> _allBrands;
        private ProductSearch _uniquePage;
        private MediaCategory _productGalleriesCategory;
        private HashSet<ProductOption> _productOptions;


        public ImportProductsService(IDocumentService documentService, IBrandService brandService,
             IImportProductSpecificationsService importSpecificationsService, IImportProductVariantsService importProductVariantsService,
            IImportProductImagesService importProductImagesService, IImportProductUrlHistoryService importUrlHistoryService, ISession session)
        {
            _documentService = documentService;
            _brandService = brandService;
            _importSpecificationsService = importSpecificationsService;
            _importProductVariantsService = importProductVariantsService;
            _importProductImagesService = importProductImagesService;
            _importUrlHistoryService = importUrlHistoryService;
            _session = session;
        }

        public void SetAllDocuments(IEnumerable<Document> documents)
        {
            _allDocuments = new HashSet<Document>(documents);
        }

        public IImportProductsService Initialize()
        {
            _allDocuments = new HashSet<Document>(_documentService.GetAllDocuments<Document>());
            _allBrands = new HashSet<Brand>(_brandService.GetAll());
            _importSpecificationsService.Initialize();
            _importProductVariantsService.Initialize();
            _importUrlHistoryService.Initialize();
            _productOptions = new HashSet<ProductOption>(_session.QueryOver<ProductOption>().List());
            return this;
        }

        /// <summary>
        /// Do import
        /// </summary>
        /// <param name="productsToImport"></param>
        public void ImportProductsFromDTOs(HashSet<ProductImportDataTransferObject> productsToImport)
        {
            _uniquePage = _documentService.GetUniquePage<ProductSearch>();
            _productGalleriesCategory = _documentService.GetDocumentByUrl<MediaCategory>("product-galleries");
            if (_productGalleriesCategory == null)
            {
                _productGalleriesCategory = new MediaCategory
                {
                    Name = "Product Galleries",
                    UrlSegment = "product-galleries",
                    IsGallery = true,
                    HideInAdminNav = true
                };
                _documentService.AddDocument(_productGalleriesCategory);
            }

            _session.Transact(session =>
                {
                    foreach (var dataTransferObject in productsToImport)
                    {
                        var transferObject = dataTransferObject;
                        ImportProduct(transferObject);
                    }
                    _allDocuments.ForEach(session.SaveOrUpdate);
                });

            foreach (var dataTransferObject in productsToImport)
            {
                var product =
                    _allDocuments.OfType<Product>().FirstOrDefault(p => p.UrlSegment == dataTransferObject.UrlSegment);

                if (product != null)
                    _importProductImagesService.ImportProductImages(dataTransferObject.Images, product.Gallery);
            }
        }

        /// <summary>
        /// Import from DTOs
        /// </summary>
        /// <param name="dataTransferObject"></param>
        public Product ImportProduct(ProductImportDataTransferObject dataTransferObject)
        {
            if (_allDocuments == null)
                _allDocuments = new HashSet<Document>();
            var product =
                _allDocuments.OfType<Product>()
                             .SingleOrDefault(x => x.UrlSegment == dataTransferObject.UrlSegment) ??
                             new Product();

            product.Parent = _uniquePage;
            product.UrlSegment = dataTransferObject.UrlSegment;
            product.Name = dataTransferObject.Name;
            product.BodyContent = dataTransferObject.Description;
            product.MetaTitle = dataTransferObject.SEOTitle;
            product.MetaDescription = dataTransferObject.SEODescription;
            product.MetaKeywords = dataTransferObject.SEOKeywords;
            product.Abstract = dataTransferObject.Abstract;
            product.PublishOn = dataTransferObject.PublishDate;

            var isNew = false;
            var productGallery = product.Gallery ?? new MediaCategory();
            if (product.Id == 0)
            {
                isNew = true;
                product.DisplayOrder = _allDocuments.Count(webpage => webpage.Parent == _uniquePage);
                productGallery.Name = product.Name;
                productGallery.UrlSegment = "product-galleries/" + product.UrlSegment;
                productGallery.IsGallery = true;
                productGallery.Parent = _productGalleriesCategory;
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
                _allDocuments.Add(product);
                _allDocuments.Add(productGallery);
            }

            return product;
        }

        private void SetBrand(ProductImportDataTransferObject dataTransferObject, Product product)
        {
            //Brand
            if (!String.IsNullOrWhiteSpace(dataTransferObject.Brand))
            {
                var dtoBrand = dataTransferObject.Brand.Trim();
                var brand = _allBrands.SingleOrDefault(x => x.Name == dtoBrand);
                if (brand == null)
                {
                    brand = new Brand { Name = dtoBrand };
                    _allBrands.Add(brand);
                }
                product.Brand = brand;
            }
        }

        private void SetOptions(ProductImportDataTransferObject dataTransferObject, Product product)
        {
            var optionsToAdd =
                dataTransferObject.Options.Where(
                    s => !product.Options.Select(option => option.Name).Contains(s, StringComparer.OrdinalIgnoreCase))
                                  .ToList();
            var optionsToRemove =
                product.Options.Where(option => !dataTransferObject.Options.Contains(option.Name)).ToList();

            foreach (var option in optionsToAdd)
            {
                var existingOption =
                    _productOptions.FirstOrDefault(
                        productOption => productOption.Name.Equals(option, StringComparison.OrdinalIgnoreCase));
                if (existingOption == null)
                {
                    existingOption = new ProductOption
                        {
                            Name = option,
                        };

                    _productOptions.Add(existingOption);
                    _session.Save(existingOption);
                }
                product.Options.Add(existingOption);
                existingOption.Products.Add(product);
            }
            foreach (var option in optionsToRemove)
            {
                product.Options.Remove(option);
                option.Products.Remove(product);
            }
        }

        public void SetCategories(ProductImportDataTransferObject dataTransferObject, Product product)
        {
            //Categories
            var categoriesToAdd =
                dataTransferObject.Categories.Where(
                    s =>
                    !product.Categories.Select(category => category.UrlSegment)
                            .Contains(s, StringComparer.OrdinalIgnoreCase)).ToList();
            var categoriesToRemove =
                product.Categories.Where(
                    category => !dataTransferObject.Categories.Contains(category.UrlSegment, StringComparer.OrdinalIgnoreCase))
                       .ToList();
            foreach (var item in categoriesToAdd)
            {
                var category = _allDocuments.OfType<Category>().SingleOrDefault(x => x.UrlSegment == item);
                if (category != null)
                {
                    product.Categories.Add(category);
                    if (!category.Products.Contains(product))
                        category.Products.Add(product);
                }
            }
            foreach (var category in categoriesToRemove)
            {
                product.Categories.Remove(category);
                if (category.Products.Contains(product))
                    category.Products.Remove(product);
            }
        }
    }
}