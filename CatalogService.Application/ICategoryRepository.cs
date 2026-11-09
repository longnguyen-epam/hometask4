using CatalogService.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Application
{
    public interface ICategoryRepository
    {
        Task<Category?> GetAsync(int id);
        Task<List<Category>> ListAsync();
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
    }
}
