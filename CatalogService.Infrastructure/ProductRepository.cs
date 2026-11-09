using CatalogService.Application;
using CatalogService.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Infrastructure
{
    public class ProductRepository(CatalogDbContext db) : IProductRepository
    {
        public Task<Product?> GetAsync(int id) =>
            db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        public Task<List<Product>> ListAsync() =>
            db.Products.AsNoTracking().OrderBy(p => p.Id).ToListAsync();

        public async Task AddAsync(Product product)
        {
            db.Products.Add(product);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            db.Products.Update(product);
            await db.SaveChangesAsync();
        }

        public Task DeleteAsync(int id) =>
            db.Products.Where(p => p.Id == id).ExecuteDeleteAsync();
    }
}
