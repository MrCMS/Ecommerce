using System.Collections.Generic;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Pages.ProductTests
{
    public class ProductVariantsTests
    {
        [Fact]
        public void Product_IsMultiVariant_FalseIfNoVariants()
        {
            var product = new Product();

            var isMultiVariant = product.IsMultiVariant;

            isMultiVariant.Should().BeFalse();
        }
        [Fact]
        public void Product_IsMultiVariant_FalseIf1Variant()
        {
            var product = new Product();
            product.Variants.Add(new ProductVariant());

            var isMultiVariant = product.IsMultiVariant;

            isMultiVariant.Should().BeFalse();
        }

        [Fact]
        public void Product_HasVariants_TrueIfMoreThan1Variants()
        {
            var product = new Product();
            product.Variants.Add(new ProductVariant());
            product.Variants.Add(new ProductVariant());

            var hasVariants = product.IsMultiVariant;

            hasVariants.Should().BeTrue();
        }
    }
}