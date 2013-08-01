using System;
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
        private readonly IProductVariantService _productVariantService;
        private List<Document> _allDocuments;
        private IList<ProductVariant> _allVariants;
        private IList<Brand> _allBrands;
        private ProductSearch _uniquePage;
        private MediaCategory _productGalleriesCategory;


        public ImportProductsService(IDocumentService documentService, IBrandService brandService,
             IImportProductSpecificationsService importSpecificationsService, IImportProductVariantsService importProductVariantsService,
            IImportProductImagesService importProductImagesService, IImportProductUrlHistoryService importUrlHistoryService, ISession session, IProductVariantService productVariantService)
        {
            _documentService = documentService;
            _brandService = brandService;
            _importSpecificationsService = importSpecificationsService;
            _importProductVariantsService = importProductVariantsService;
            _importProductImagesService = importProductImagesService;
            _importUrlHistoryService = importUrlHistoryService;
            _session = session;
            _productVariantService = productVariantService;

            _allDocuments=new List<Document>();
            _allVariants=new List<ProductVariant>();
            _allBrands=new List<Brand>();
        }

        /// <summary>
        /// Do import
        /// </summary>
        /// <param name="productsToImport"></param>
        public void ImportProductsFromDTOs(IEnumerable<ProductImportDataTransferObject> productsToImport)
        {
            _allDocuments = _documentService.GetAllDocuments<Document>().ToList();
            _allVariants = _productVariantService.GetAll();
            _allBrands = _brandService.GetAll();
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
            if(_allDocuments==null)
                _allDocuments=new List<Document>();
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

            //Categories
            product.Categories.Clear();
            foreach (var item in dataTransferObject.Categories)
            {
                var category = _allDocuments.OfType<Category>().SingleOrDefault(x => x.UrlSegment == item);
                if (category != null && product.Categories.All(x => x.Id != category.Id))
                {
                    product.Categories.Add((Category)category);
                }
            }

            product.AttributeOptions.Clear();

            ////Url History
            _importUrlHistoryService.ImportUrlHistory(dataTransferObject, product);

            ////Specifications
            _importSpecificationsService.ImportSpecifications(dataTransferObject, product);

            ////Variants
            _importProductVariantsService.ImportVariants(dataTransferObject, product);

            if (product.Id == 0)
            {
                product.DisplayOrder = _allDocuments.Count(webpage => webpage.Parent == _uniquePage);
                var productGallery = new MediaCategory
                {
                    Name = product.Name,
                    UrlSegment = "product-galleries/" + product.UrlSegment,
                    IsGallery = true,
                    Parent = _productGalleriesCategory,
                    HideInAdminNav = true
                };
                product.Gallery = productGallery;

                _allDocuments.Add(product);
                _allDocuments.Add(productGallery);
            }

            return product;
        }

    }
}