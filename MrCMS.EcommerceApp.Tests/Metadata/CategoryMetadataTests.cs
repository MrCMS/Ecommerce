using System;
using System.Collections.Generic;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Web.Apps.Ecommerce.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Metadata
{
    public class CategoryMetadataTests
    {
        [Fact]
        public void CategoryMetadata_ChildrenListType_ShouldBeWhiteList()
        {
            var metadata = new CategoryMetadata();

            metadata.ChildrenListType.Should().Be(ChildrenListType.WhiteList);
        }

        [Fact]
        public void CategoryMetadata_ChildrenList_ShouldBeJustCategories()
        {
            var metadata = new CategoryMetadata();

            metadata.ChildrenList.Should().BeEquivalentTo(new List<Type>{typeof(Category)});
        }

        [Fact]
        public void CategoryMetadata_RequiresParent_ShouldBeTrue()
        {
            var metadata = new CategoryMetadata();

            metadata.RequiresParent.Should().BeTrue();
        }

        [Fact]
        public void CategoryMetadata_AutoBlacklist_ShouldBeTrue()
        {
            var metadata = new CategoryMetadata();

            metadata.AutoBlacklist.Should().BeTrue();
        }
    }
}