using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
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
        private IList<Webpage> _allDocuments;
        private IList<ProductVariant> _allVariants;
        private IList<Brand> _allBrands;
        private ProductSearch _uniquePage;


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
            
        }

        /// <summary>
        /// Do import
        /// </summary>
        /// <param name="productsToImport"></param>
        public void ImportProductsFromDTOs(IEnumerable<ProductImportDataTransferObject> productsToImport)
        {
            _allDocuments = _documentService.GetAllDocuments<Webpage>().ToList();
            _allVariants = _productVariantService.GetAll();
            _allBrands = _brandService.GetAll();
            _uniquePage = _documentService.GetUniquePage<ProductSearch>();

            foreach (var dataTransferObject in productsToImport)
            {
                ProductImportDataTransferObject transferObject = dataTransferObject;
                _session.Transact(session =>
                    {
                        ImportProduct(transferObject);
                    });
            }
        }

        /// <summary>
        /// Import from DTOs
        /// </summary>
        /// <param name="dataTransferObject"></param>
        public Product ImportProduct(ProductImportDataTransferObject dataTransferObject)
        {
            var product = (Product)_allDocuments.SingleOrDefault(x=>x.UrlSegment == dataTransferObject.UrlSegment && x.DocumentType == "Product") ?? new Product();

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
                var category = _allDocuments.SingleOrDefault(x => x.UrlSegment == item && x.DocumentType == "Category");
                if (category != null && product.Categories.All(x => x.Id != category.Id))
                {
                    product.Categories.Add((Category)category);
                }
            }

            product.AttributeOptions.Clear();
            
            ////Url History
            //_importUrlHistoryService.ImportUrlHistory(dataTransferObject, product);


            ////Specifications
            //_importSpecificationsService.ImportSpecifications(dataTransferObject, product);

            ////Variants
            //_importProductVariantsService.ImportVariants(dataTransferObject, product);
            
            if (product.Id == 0)
            {
                _documentService.AddDocument(product);
                _allDocuments.Add(product);
            }
            else
                _documentService.SaveDocument(product);

            ////Images
            ////_importProductImagesService.ImportProductImages(dataTransferObject.Images, product.Gallery);

            //_documentService.SaveDocument(product);

            return product;
        }

    }
}