using System.Collections.Generic;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Pages;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Pages.ProductTests
{
    public class ProductVariantsTests
    {
        [Fact]
        public void Product_HasVariants_FalseIfNoVariants()
        {
            var product = new Product();

            var hasVariants = product.HasVariants;

            hasVariants.Should().BeFalse();
        }

        [Fact]
        public void Product_HasVariants_TrueIfAnyVariants()
        {
            var product = new Product();
            product.Variants.Add(new ProductVariant());

            var hasVariants = product.HasVariants;

            hasVariants.Should().BeTrue();
        }
    }
}