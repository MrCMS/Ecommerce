using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Pages.ProductTests
{
    public class SpecificationTests
    {
        [Fact]
        public void Product_GetSpecification_ReturnsNullWhenThereIsNoRelevantValue()
        {
            var product = new Product();

            var specification = product.GetSpecification("Test");

            specification.Should().BeNull();
        }

        [Fact]
        public void Product_GetSpecification_ReturnsValueWhenItExists()
        {
            var product = new Product();
            product.SpecificationValues.Add(new ProductSpecificationValue
                                                {
                                                    Value = "value",
                                                    Option =
                                                        new ProductSpecificationOption
                                                            {
                                                                Name = "Test"
                                                            }
                                                });

            var specification = product.GetSpecification("Test");

            specification.Should().Be("value");
        }
    }
}