using CatalogService.Domain;

namespace CatalogService.Application
{
    public interface IProductRepository
    {
        Task<Product?> GetAsync(int id);
        Task<List<Product>> ListAsync();
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }

}
