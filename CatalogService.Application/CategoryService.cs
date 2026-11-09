using CatalogService.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Application
{
    public class CategoryService(ICategoryRepository categories)
    {
        public Task<Category?> GetAsync(int id) => categories.GetAsync(id);

        public Task<List<Category>> ListAsync() => categories.ListAsync();

        public async Task AddAsync(Category category)
        {
            await ValidateAsync(category);
            await categories.AddAsync(category);
        }

        public async Task UpdateAsync(Category category)
        {
            await ValidateAsync(category);
            await categories.UpdateAsync(category);
        }

        // Products of the category are deleted by the database (cascade); subcategories become root categories.
        public Task DeleteAsync(int id) => categories.DeleteAsync(id);

        private async Task ValidateAsync(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name)) throw new ArgumentException("Name is required.", nameof(category));
            if (category.Name.Length > 50) throw new ArgumentException("Name max length is 50.", nameof(category));
            if (category.ImageUrl is not null && !Uri.IsWellFormedUriString(category.ImageUrl, UriKind.Absolute))
                throw new ArgumentException("Image URL must be absolute.", nameof(category));

            if (category.ParentCategoryId is int parentId)
            {
                if (parentId == category.Id) throw new ArgumentException("Category cannot be its own parent.", nameof(category));
                if (await categories.GetAsync(parentId) is null) throw new ArgumentException("Parent category not found.", nameof(category));
            }
        }
    }
}
