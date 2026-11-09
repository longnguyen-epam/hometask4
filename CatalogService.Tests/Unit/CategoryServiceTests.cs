using CatalogService.Application;
using CatalogService.Domain;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Tests.Unit
{
    public class CategoryServiceTests
    {
        private readonly ICategoryRepository _categories = Substitute.For<ICategoryRepository>();
        private readonly CategoryService _service;

        public CategoryServiceTests() => _service = new CategoryService(_categories);

        [Fact]
        public async Task AddAsync_saves_valid_category()
        {
            var category = new Category { Name = "Books" };

            await _service.AddAsync(category);

            await _categories.Received(1).AddAsync(category);
        }

        [Theory]
        [InlineData("")]
        [InlineData("This name is definitely longer than fifty characters!")]
        public async Task AddAsync_rejects_invalid_name(string name)
        {
            await Should.ThrowAsync<ArgumentException>(() => _service.AddAsync(new Category { Name = name }));
            await _categories.DidNotReceiveWithAnyArgs().AddAsync(default!);
        }

        [Fact]
        public async Task AddAsync_rejects_missing_parent()
        {
            var category = new Category { Name = "Novels", ParentCategoryId = 42 };

            await Should.ThrowAsync<ArgumentException>(() => _service.AddAsync(category));
        }

        [Fact]
        public async Task UpdateAsync_rejects_category_as_its_own_parent()
        {
            var category = new Category { Id = 1, Name = "Books", ParentCategoryId = 1 };

            await Should.ThrowAsync<ArgumentException>(() => _service.UpdateAsync(category));
        }
    }

}
