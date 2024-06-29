using CandyKeeper.Domain.Models;

namespace CandyKeeper.DAL
{
    public interface IProductRepository
    {
        Task Create(Product product);
        Task Delete(int id);
        Task<List<Product>> Get();
        Task<Product> GetById(int id);
        Task Update(int id, string name, int productTypeId);
    }
}