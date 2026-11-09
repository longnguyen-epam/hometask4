using CatalogService.Application;
using CatalogService.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Infrastructure
{
    public class CategoryRepository(CatalogDbContext db) : ICategoryRepository
    {
        public Task<Category?> GetAsync(int id) =>
            db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

        public Task<List<Category>> ListAsync() =>
            db.Categories.AsNoTracking().OrderBy(c => c.Id).ToListAsync();

        public async Task AddAsync(Category category)
        {
            db.Categories.Add(category);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            db.Categories.Update(category);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await db.Categories
                .Where(c => c.ParentCategoryId == id)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.ParentCategoryId, (int?)null));
            await db.Categories.Where(c => c.Id == id).ExecuteDeleteAsync();
        }
    }
}
