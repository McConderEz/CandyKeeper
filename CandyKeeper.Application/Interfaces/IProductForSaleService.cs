using CandyKeeper.Domain.Models;

namespace CandyKeeper.Application.Interfaces
{
    public interface IProductForSaleService
    {
        Task Create(ProductForSale productForSale);
        Task Delete(int id);
        Task<List<ProductForSale>> Get();
        Task<ProductForSale> GetById(int id);
        Task Update(ProductForSale productForSale);
        Task<List<ProductForSale>> GetByStoreId(int storeId);
        Task<List<ProductForSale>> GetBySearchingString(string searchingString);
    }
}