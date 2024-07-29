using CandyKeeper.Domain.Models;

namespace CandyKeeper.Application.Interfaces
{
    public interface IProductTypeService
    {
        Task Create(ProductType productType);
        Task Delete(int id);
        Task<List<ProductType>> Get();
        Task<ProductType> GetById(int id);
        Task Update(ProductType productType);
        Task<List<ProductType>> GetBySearchingString(string searchingString);
    }
}