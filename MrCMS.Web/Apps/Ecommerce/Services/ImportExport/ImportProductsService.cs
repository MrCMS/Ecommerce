using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductsService : IImportProductsService
    {
        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariantService;
        private readonly IDocumentService _documentService;
        private readonly IProductOptionManager _productOptionManager;
        private readonly IBrandService _brandService;
        private readonly ITaxRateManager _taxRateManager;
        private readonly IFileService _fileService;

        public ImportProductsService(IProductService productService, IProductVariantService productVariantService,
            IDocumentService documentService, IProductOptionManager productOptionManager, IBrandService brandService, ITaxRateManager taxRateManager,
            IFileService fileService)
        {
            _productService = productService;
            _productVariantService = productVariantService;
            _documentService = documentService;
            _productOptionManager = productOptionManager;
            _brandService = brandService;
            _taxRateManager = taxRateManager;
            _fileService = fileService;
        }

        /// <summary>
        /// Do import
        /// </summary>
        /// <param name="productsToImport"></param>
        public void ImportProductsFromDTOs(IEnumerable<ProductImportDataTransferObject> productsToImport)
        {
            foreach (var dataTransferObject in productsToImport)
            {
                ImportProduct(dataTransferObject);
            }
        }

        /// <summary>
        /// Import from DTOs
        /// </summary>
        /// <param name="dataTransferObject"></param>
        public void ImportProduct(ProductImportDataTransferObject dataTransferObject)
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
            product.PublishOn = DateTime.UtcNow;

            //Brand
            if (!_brandService.AnyExistingBrandsWithName(dataTransferObject.Brand, 0))
                _brandService.Add(new Brand() { Name = dataTransferObject.Brand });
            var brandEntity = _brandService.GetBrandByName(dataTransferObject.Brand);
            if (brandEntity != null)
                product.Brand = brandEntity;

            //Categories
            product.Categories.Clear();
            foreach (var item in dataTransferObject.Categories)
            {
                var category = _documentService.GetDocument<Category>(item);
                if (category != null && !product.Categories.Any(x => x.Id == category.Id))
                    product.Categories.Add(category);
            }

            product.AttributeOptions.Clear();

            if (product.Id == 0)
            {
                _documentService.AddDocument<Product>(product);
                product.Variants.Clear();
            }
            else
                _documentService.SaveDocument<Product>(product);

            product = _productService.Get(product.Id);

            //Images
            ImportProductImages(dataTransferObject, product);

            //Specifications
            ImportSpecifications(dataTransferObject, product);

            //Variants
            ImportVariants(dataTransferObject, product);
        }

        /// <summary>
        /// Add Specifications
        /// </summary>
        /// <param name="dataTransferObject"></param>
        /// <param name="product"></param>
        public void ImportSpecifications(ProductImportDataTransferObject dataTransferObject, Product product)
        {
            product.SpecificationValues.Clear();
            foreach (var item in dataTransferObject.Specifications)
            {
                if (!_productOptionManager.AnyExistingSpecificationAttributesWithName(item.Key))
                    _productOptionManager.AddSpecificationAttribute(new ProductSpecificationAttribute() { Name = item.Key });
                var option = _productOptionManager.GetSpecificationAttributeByName(item.Key);
                if (!option.Options.Any(x => x.Name == item.Value))
                {
                    option.Options.Add(new ProductSpecificationAttributeOption()
                    {
                        ProductSpecificationAttribute = option,
                        Name = item.Value
                    });
                    _productOptionManager.UpdateSpecificationAttribute(option);
                }
                var optionValue = option.Options.SingleOrDefault(x => x.Name == item.Value);
                if (
                    !product.SpecificationValues.Any(
                        x =>
                        optionValue != null &&
                        (x.ProductSpecificationAttributeOption.Id == optionValue.Id && x.Product.Id == product.Id)))
                    product.SpecificationValues.Add(new ProductSpecificationValue()
                    {
                        ProductSpecificationAttributeOption = optionValue,
                        Product = product,
                    });
            }

            _documentService.SaveDocument(product);
        }

        /// <summary>
        /// Add Variants
        /// </summary>
        /// <param name="dataTransferObject"></param>
        /// <param name="product"></param>
        public void ImportVariants(ProductImportDataTransferObject dataTransferObject, Product product)
        {
            foreach (var item in dataTransferObject.ProductVariants)
            {
                var productVariant = _productVariantService.GetProductVariantBySKU(item.SKU) ?? new ProductVariant();

                productVariant.Name = item.Name;
                productVariant.SKU = item.SKU;
                productVariant.Barcode = item.Barcode;
                productVariant.BasePrice = item.Price;
                productVariant.PreviousPrice = item.PreviousPrice;
                productVariant.StockRemaining = item.Stock;
                productVariant.Weight = item.Weight.HasValue ? item.Weight.Value : 0;
                productVariant.TrackingPolicy = item.TrackingPolicy;
                productVariant.TaxRate = _taxRateManager.Get(item.TaxRate.HasValue ? item.TaxRate.Value : 0);
                productVariant.Product = product;

                for (var i = productVariant.AttributeValues.Count - 1; i >= 0; i--)
                {
                    var value = productVariant.AttributeValues[i];
                    productVariant.AttributeValues.Remove(value);
                    _productOptionManager.DeleteProductAttributeValue(value);
                }

                _productVariantService.Update(productVariant);

                productVariant = _productVariantService.GetProductVariantBySKU(item.SKU);

                foreach (var opt in item.Options)
                {
                    var option = _productOptionManager.GetAttributeOptionByName(opt.Key);
                    if (option == null)
                        _productOptionManager.AddAttributeOption(new ProductAttributeOption() { Name = opt.Key });
                    option = _productOptionManager.GetAttributeOptionByName(opt.Key);
                    if (!productVariant.Product.AttributeOptions.Any(x => x.Id == option.Id))
                        product.AttributeOptions.Add(option);
                    if (!productVariant.AttributeValues.Any(x => x.ProductAttributeOption.Id == option.Id))
                        productVariant.AttributeValues.Add(new ProductAttributeValue()
                        {
                            ProductAttributeOption = option,
                            ProductVariant = productVariant,
                            Value = opt.Value
                        });
                    else
                    {
                        var productAttributeValue =
                            productVariant.AttributeValues.SingleOrDefault(x => x.ProductAttributeOption.Id == option.Id);
                        if (productAttributeValue != null)
                            productAttributeValue.Value = opt.Value;
                    }
                }

                _productVariantService.Update(productVariant);
            }

            _documentService.SaveDocument<Product>(product);
        }

        /// <summary>
        /// Add Product Images
        /// </summary>
        /// <param name="dataTransferObject"></param>
        /// <param name="product"></param>
        public void ImportProductImages(ProductImportDataTransferObject dataTransferObject, Product product)
        {
            if (!product.Images.Any())
            {
                foreach (var image in dataTransferObject.Images)
                {
                    if (!String.IsNullOrWhiteSpace(image))
                        ImportImageToGallery(image.Replace("?update=no", "").Replace("?update=yes", ""), product.Gallery);
                }
            }
            else
            {
                foreach (
                    var image in
                        dataTransferObject.Images.Where(
                            image => !String.IsNullOrWhiteSpace(image) && image.Contains("?update=yes")))
                {
                    ImportImageToGallery(image.Replace("?update=no", "").Replace("?update=yes", ""), product.Gallery);
                }
            }
        }

        /// <summary>
        /// Add image to Product Gallery
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <param name="mediaCategory"></param>
        public bool ImportImageToGallery(string fileLocation, MediaCategory mediaCategory)
        {
            using (var client = new WebClient())
            {
                var fileName = Path.GetFileName(fileLocation);
                try
                {
                    var downloadedFile = client.DownloadData(fileLocation);
                    if (downloadedFile != null)
                    {
                        _fileService.AddFile(new MemoryStream(downloadedFile), fileName, "image/png",
                                             downloadedFile.Length, mediaCategory);
                        return true;
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}