using CatalogService.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Application
{
    public class ProductService(IProductRepository products, ICategoryRepository categories)
    {
        public Task<Product?> GetAsync(int id) => products.GetAsync(id);

        public Task<List<Product>> ListAsync() => products.ListAsync();

        public async Task AddAsync(Product product)
        {
            await ValidateAsync(product);
            await products.AddAsync(product);
        }

        public async Task UpdateAsync(Product product)
        {
            await ValidateAsync(product);
            await products.UpdateAsync(product);
        }

        public Task DeleteAsync(int id) => products.DeleteAsync(id);

        private async Task ValidateAsync(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name)) throw new ArgumentException("Name is required.", nameof(product));
            if (product.Name.Length > 50) throw new ArgumentException("Name max length is 50.", nameof(product));
            if (product.Price < 0) throw new ArgumentException("Price cannot be negative.", nameof(product));
            if (product.Amount <= 0) throw new ArgumentException("Amount must be positive.", nameof(product));
            if (product.ImageUrl is not null && !Uri.IsWellFormedUriString(product.ImageUrl, UriKind.Absolute))
                throw new ArgumentException("Image URL must be absolute.", nameof(product));
            if (await categories.GetAsync(product.CategoryId) is null)
                throw new ArgumentException("Category not found.", nameof(product));
        }
    }

}
