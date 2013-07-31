using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Do import
        /// </summary>
        /// <param name="productsToImport"></param>
        public void ImportProductsFromDTOs(IEnumerable<ProductImportDataTransferObject> productsToImport)
        {
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
            var product = _documentService.GetDocumentByUrl<Product>(dataTransferObject.UrlSegment) ?? new Product();

            product.Parent = _documentService.GetUniquePage<ProductSearch>();
            product.UrlSegment = dataTransferObject.UrlSegment;
            product.Name = dataTransferObject.Name;
            product.BodyContent = dataTransferObject.Description;
            product.MetaTitle = dataTransferObject.SEOTitle;
            product.MetaDescription = dataTransferObject.SEODescription;
            product.MetaKeywords = dataTransferObject.SEOKeywords;
            product.Abstract = dataTransferObject.Abstract;

            //Brand
            if (!String.IsNullOrWhiteSpace(dataTransferObject.Brand))
            {
                var brand = _brandService.GetBrandByName(dataTransferObject.Brand);
                if (brand == null)
                {
                    brand = new Brand { Name = dataTransferObject.Brand };
                }
                product.Brand = brand;
            }

            //Categories
            product.Categories.Clear();
            foreach (var item in dataTransferObject.Categories)
            {
                var category = _documentService.GetDocumentByUrl<Category>(item);
                if (category != null && product.Categories.All(x => x.Id != category.Id))
                    product.Categories.Add(category);
            }

            product.AttributeOptions.Clear();
            
            //Url History
            _importUrlHistoryService.ImportUrlHistory(dataTransferObject, product);


            //Specifications
            _importSpecificationsService.ImportSpecifications(dataTransferObject, product);

            //Variants
            _importProductVariantsService.ImportVariants(dataTransferObject, product);
            
            if (product.Id == 0)
            {
                _documentService.AddDocument(product);
            }
            else
                _documentService.SaveDocument(product);

            //Images
            _importProductImagesService.ImportProductImages(dataTransferObject.Images, product.Gallery);

            _documentService.SaveDocument(product);

            return product;
        }

    }
}