using CandyKeeper.Domain.Models;

namespace CandyKeeper.Application.Interfaces
{
    public interface IProductService
    {
        Task Create(Product product);
        Task Delete(int id);
        Task<List<Product>> Get();
        Task<Product> GetById(int id);
        Task Update(Product product);
        Task<List<Product>> GetBySearchingString(string searchingString);
    }
}