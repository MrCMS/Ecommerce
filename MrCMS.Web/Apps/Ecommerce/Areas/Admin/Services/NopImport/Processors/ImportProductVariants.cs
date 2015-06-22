using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportProductVariants : IImportProductVariants
    {
        public IEnumerable<ProductVariant> CreateProductVariants(NopImportContext nopImportContext, HashSet<ProductVariantData> productVariants, HashSet<ProductOptionValueData> optionValues, Product product)
        {
            foreach (ProductVariantData variantData in productVariants)
            {
                HashSet<ProductOptionValueData> productOptionValueDatas =
                    optionValues.Where(data => data.VariantId == variantData.Id).ToHashSet();
                if (productOptionValueDatas.Any())
                {
                    foreach (var variant in CreateGroupedProductVariants(nopImportContext, productOptionValueDatas, variantData, product))
                    {
                        yield return variant;
                    }
                }
                else
                {
                   yield return CreateStandardProductVariant(nopImportContext, variantData, product);
                }
            }

        }

        private static ProductVariant CreateStandardProductVariant(NopImportContext nopImportContext, ProductVariantData variantData,
            Product product)
        {
            var variant = new ProductVariant
            {
                Name = variantData.Name,
                SKU = variantData.SKU,
                Weight = variantData.Weight,
                TrackingPolicy = variantData.TrackingPolicy,
                StockRemaining = variantData.StockRemaining,
                RequiresShipping = variantData.RequiresShipping,
                AllowedNumberOfDaysForDownload = variantData.DownloadDays,
                AllowedNumberOfDownloads = variantData.MaxDownloads,
                IsDownloadable = variantData.Download,
                BasePrice = variantData.BasePrice,
                DownloadFileUrl = variantData.DownloadUrl,
                GiftCardType = variantData.GiftCardType,
                IsGiftCard = variantData.GiftCard,
                Product = product,
                PreviousPrice = variantData.PreviousPrice,
                ManufacturerPartNumber = variantData.PartNumber,
                TaxRate = nopImportContext.FindNew<TaxRate>(variantData.TaxRate),
                PriceBreaks = variantData.PriceBreaks.Select(price => new PriceBreak
                {
                    Price = price.Price,
                    Quantity = price.Quantity
                }).ToList()
            };
            if (variantData.PriceBreaks.Count > 0)
            {
                product.Variants.Add(variant);
            }
            else
            {
                product.Variants.Add(variant);
            }
            nopImportContext.AddEntry(variantData.Id, variant);
            return variant;
        }

        private static IEnumerable<ProductVariant> CreateGroupedProductVariants(NopImportContext nopImportContext, HashSet<ProductOptionValueData> productOptionValueDatas,
            ProductVariantData variantData, Product product)
        {
            IEnumerable<IGrouping<int, ProductOptionValueData>> groups =
                productOptionValueDatas.GroupBy(data => data.OptionId);

            var cartesianProductList =
                CartesianProduct(groups).ToList();

            for (int index = 0; index < cartesianProductList.Count; index++)
            {
                var cartesianProduct = cartesianProductList[index];
                yield return
                    CreateCartesianProductVariant(nopImportContext, variantData, product, cartesianProduct, index);
            }
        }

        private static ProductVariant CreateCartesianProductVariant(NopImportContext nopImportContext, ProductVariantData variantData, Product product, IEnumerable<ProductOptionValueData> cartesianProduct, int index)
        {
            var variant = new ProductVariant
            {
                Name = variantData.Name,
                SKU = variantData.SKU + "-" + (index + 1),
                Weight = variantData.Weight,
                TrackingPolicy = variantData.TrackingPolicy,
                StockRemaining = variantData.StockRemaining,
                RequiresShipping = variantData.RequiresShipping,
                AllowedNumberOfDaysForDownload = variantData.DownloadDays,
                AllowedNumberOfDownloads = variantData.MaxDownloads,
                IsDownloadable = variantData.Download,
                BasePrice = variantData.BasePrice,
                DownloadFileUrl = variantData.DownloadUrl,
                GiftCardType = variantData.GiftCardType,
                IsGiftCard = variantData.GiftCard,
                Product = product,
                PreviousPrice = variantData.PreviousPrice,
                ManufacturerPartNumber = variantData.PartNumber,
                TaxRate = nopImportContext.FindNew<TaxRate>(variantData.TaxRate)
            };
            foreach (ProductOptionValueData valueData in cartesianProduct)
            {
                variant.Weight += valueData.WeightAdjustment;
                variant.BasePrice += valueData.PriceAdjustment;

                var productOptionValue = new ProductOptionValue
                {
                    ProductVariant = variant,
                    Value = valueData.Value,
                    ProductOption = nopImportContext.FindNew<ProductOption>(valueData.OptionId)
                };
                variant.OptionValues.Add(productOptionValue);
                if (!product.Options.Contains(productOptionValue.ProductOption))
                    product.Options.Add(productOptionValue.ProductOption);
                nopImportContext.AddEntry(valueData.Id, productOptionValue);
            }

            product.Variants.Add(variant);
            nopImportContext.AddEntry(variantData.Id, variant);

            return variant;
        }


        private static IEnumerable<IEnumerable<T>> CartesianProduct<T>(IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(
                emptyProduct,
                (accumulator, sequence) =>
                    from accseq in accumulator
                    from item in sequence
                    select accseq.Concat(new[] { item }));
        }
    }
}