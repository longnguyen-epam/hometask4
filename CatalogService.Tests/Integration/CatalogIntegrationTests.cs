using CatalogService.Application;
using CatalogService.Infrastructure;
using EmptyFiles;
using System;
using System.Collections.Generic;
using System.Text;
using CatalogService.Domain;
using Category = CatalogService.Domain.Category;
using Shouldly;

namespace CatalogService.Tests.Integration
{
    public sealed class CatalogIntegrationTests(CatalogDbFixture fixture) : IClassFixture<CatalogDbFixture>, IDisposable
    {
        private readonly CatalogDbContext _db = fixture.CreateContext();

        private CategoryService Categories => new(new CategoryRepository(_db));
        private ProductService Products => new(new ProductRepository(_db), new CategoryRepository(_db));

        public void Dispose() => _db.Dispose();

        [Fact]
        public async Task Category_and_product_can_be_added_updated_and_read()
        {
            var category = new Category { Name = "Books", ImageUrl = "https://img.example.com/books.png" };
            await Categories.AddAsync(category);

            var product = new Product { Name = "Clean Code", Description = "<b>Classic</b>", CategoryId = category.Id, Price = 39.99m, Amount = 3 };
            await Products.AddAsync(product);

            product.Amount = 10;
            await Products.UpdateAsync(product);

            var loaded = await Products.GetAsync(product.Id);
            loaded.ShouldNotBeNull();
            loaded.Amount.ShouldBe(10);
            loaded.Price.ShouldBe(39.99m);
            loaded.Description.ShouldBe("<b>Classic</b>");
        }

        [Fact]
        public async Task Deleting_category_deletes_its_products_and_keeps_subcategories_as_roots()
        {
            var parent = new Category { Name = "Electronics" };
            await Categories.AddAsync(parent);
            var child = new Category { Name = "Phones", ParentCategoryId = parent.Id };
            await Categories.AddAsync(child);
            var product = new Product { Name = "TV", CategoryId = parent.Id, Price = 500m, Amount = 1 };
            await Products.AddAsync(product);

            await Categories.DeleteAsync(parent.Id);

            (await Categories.GetAsync(parent.Id)).ShouldBeNull();
            (await Products.GetAsync(product.Id)).ShouldBeNull();
            var reloadedChild = await Categories.GetAsync(child.Id);
            reloadedChild.ShouldNotBeNull();
            reloadedChild.ParentCategoryId.ShouldBeNull();
        }
    }
}
