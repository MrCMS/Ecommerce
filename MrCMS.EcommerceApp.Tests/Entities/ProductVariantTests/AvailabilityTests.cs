using System;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;
using FluentAssertions;

namespace MrCMS.EcommerceApp.Tests.Entities.ProductVariantTests
{
    public class AvailabilityTests
    {
        [Fact]
        public void Product_Availability_IfPublishDateNotSetShouldBeOnPreOrder()
        {
            var variant = new ProductVariant();

            var availabilityStatus = variant.Availability;

            availabilityStatus.Should().Be(ProductAvailability.PreOrder);
        }

        [Fact]
        public void Product_Availability_IfPublishDateSetButInThePastShouldBeAvailable()
        {
            var product = new ProductVariant { AvailableOn = DateTime.Today.AddDays(-1) };

            var availabilityStatus = product.Availability;

            availabilityStatus.Should().Be(ProductAvailability.Available);
        }

        [Fact]
        public void Product_Availability_IfPublishDateSetButInTheFutureShouldBePreOrder()
        {
            var product = new ProductVariant { AvailableOn = DateTime.Today.AddDays(1) };

            var availabilityStatus = product.Availability;

            availabilityStatus.Should().Be(ProductAvailability.PreOrder);
        }
    }
}