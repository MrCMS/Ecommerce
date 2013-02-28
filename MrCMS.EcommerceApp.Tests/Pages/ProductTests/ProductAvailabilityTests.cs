using System;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Pages.ProductTests
{
    public class ProductAvailabilityTests
    {
        [Fact]
        public void Product_Availability_IfPublishDateNotSetShouldBeOnPreOrder()
        {
            var product = new Product();

            ProductAvailability availabilityStatus = product.Availability;

            availabilityStatus.Should().Be(ProductAvailability.PreOrder);
        }

        [Fact]
        public void Product_Availability_IfPublishDateSetButInThePastShouldBeAvailable()
        {
            var product = new Product {PublishOn = DateTime.Today.AddDays(-1)};

            ProductAvailability availabilityStatus = product.Availability;

            availabilityStatus.Should().Be(ProductAvailability.Available);
        }

        [Fact]
        public void Product_Availability_IfPublishDateSetButInTheFutureShouldBePreOrder()
        {
            var product = new Product {PublishOn = DateTime.Today.AddDays(1)};

            ProductAvailability availabilityStatus = product.Availability;

            availabilityStatus.Should().Be(ProductAvailability.PreOrder);
        }
    }
}