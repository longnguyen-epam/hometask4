using CatalogService.Application;
using CatalogService.Domain;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Tests.Unit
{
    public class ProductServiceTests
    {
        private readonly IProductRepository _products = Substitute.For<IProductRepository>();
        private readonly ICategoryRepository _categories = Substitute.For<ICategoryRepository>();
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _service = new ProductService(_products, _categories);
            _categories.GetAsync(1).Returns(new Category { Id = 1, Name = "Books" });
        }

        private static Product Product(int amount = 5, int categoryId = 1) =>
            new() { Name = "Clean Code", CategoryId = categoryId, Price = 10m, Amount = amount };

        [Fact]
        public async Task AddAsync_saves_valid_product()
        {
            var product = Product();

            await _service.AddAsync(product);

            await _products.Received(1).AddAsync(product);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task AddAsync_rejects_non_positive_amount(int amount)
        {
            await Should.ThrowAsync<ArgumentException>(() => _service.AddAsync(Product(amount: amount)));
        }

        [Fact]
        public async Task AddAsync_rejects_unknown_category()
        {
            await Should.ThrowAsync<ArgumentException>(() => _service.AddAsync(Product(categoryId: 99)));
            await _products.DidNotReceiveWithAnyArgs().AddAsync(default!);
        }
    }
}
