using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping;

namespace MrCMS.Web.Areas.Admin.Services.NopImport.Nop280
{
    public class NopCommerce280ProductReader : INopCommerceProductReader
    {
        public List<CategoryData> GetCategoryData(string connectionString)
        {
            using (Nop280DataContext context = GetContext(connectionString))
            {
                List<Category> categories = context.Categories.ToList();

                return categories.Select(category => new CategoryData
                {
                    Id = category.Id,
                    Name = category.Name,
                    ParentId = category.ParentCategoryId,
                    Abstract = category.Description
                }).ToList();
            }
        }

        public List<ProductOptionData> GetProductOptions(string connectionString)
        {

            using (Nop280DataContext context = GetContext(connectionString))
            {
                var attributes = context.SpecificationAttributes.ToList();

                return attributes.Select(attribute => new ProductOptionData
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                }).ToList();
            }
        }

        public List<ProductOptionValueData> GetProductOptionValues(string connectionString)
        {
            using (Nop280DataContext context = GetContext(connectionString))
            {
                var specificationAttributeMappings =
                    context.Product_SpecificationAttribute_Mappings.ToList();

                return specificationAttributeMappings.Select(attribute => new ProductOptionValueData
                {
                    OptionName = attribute.SpecificationAttributeOption.SpecificationAttribute.Name,
                    Value = attribute.CustomValue ?? attribute.SpecificationAttributeOption.Name,
                    ProductId = attribute.ProductId
                }).ToList();
            }
        }

        public List<ProductSpecificationData> GetProductSpecifications(string connectionString)
        {
            using (var context = GetContext(connectionString))
            {
                var productAttributes = context.ProductAttributes.ToList();

                return productAttributes.Select(attribute => new ProductSpecificationData
                {
                    Name = attribute.Name
                }).ToList();
            }
        }

        public List<ProductSpecificationValueData> GetProductSpecificationValues(string connectionString)
        {
            using (var context = GetContext(connectionString))
            {
                var mappings = context.ProductVariant_ProductAttribute_Mappings.ToList();
                var values = context.ProductVariantAttributeValues.ToList();

                return mappings.Select(mapping => new ProductSpecificationValueData
                {
                    SpecName = mapping.ProductAttribute.Name,
                }).ToList();
            }
        }

        public List<BrandData> GetBrands(string connectionString)
        {
            throw new NotImplementedException();
        }

        public List<ProductData> GetProducts(string connectionString)
        {
            throw new NotImplementedException();
        }

        private static Nop280DataContext GetContext(string connectionString)
        {
            return new Nop280DataContext(connectionString);
        }
    }
}