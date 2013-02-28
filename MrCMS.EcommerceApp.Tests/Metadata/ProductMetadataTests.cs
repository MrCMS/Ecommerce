using System;
using System.Collections.Generic;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Web.Apps.Ecommerce.Metadata;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Metadata
{
    public class ProductMetadataTests
    {
        [Fact]
        public void ProductMetadata_ChildrenListType_ShouldBeWhiteList()
        {
            var metadata = new ProductMetadata();

            metadata.ChildrenListType.Should().Be(ChildrenListType.WhiteList);
        }

        [Fact]
        public void ProductMetadata_ChildrenList_ShouldBeJustProducts()
        {
            var metadata = new ProductMetadata();

            metadata.ChildrenList.Should().BeEquivalentTo(new List<Type>());
        }

        [Fact]
        public void ProductMetadata_RequiresParent_ShouldBeTrue()
        {
            var metadata = new ProductMetadata();

            metadata.RequiresParent.Should().BeTrue();
        }

        [Fact]
        public void ProductMetadata_AutoBlacklist_ShouldBeTrue()
        {
            var metadata = new ProductMetadata();

            metadata.AutoBlacklist.Should().BeTrue();
        }
    }
}