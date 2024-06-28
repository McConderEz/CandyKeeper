using CandyKeeper.Domain.Models;

namespace CandyKeeper.DAL.Repositories
{
    public interface IProductTypeRepository
    {
        Task Create(ProductType productType);
        Task Delete(int id);
        Task<List<ProductType>> Get();
        Task<ProductType> GetById(int id);
        Task Update(int id, string name);
    }
}