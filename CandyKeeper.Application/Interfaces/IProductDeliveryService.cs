using CandyKeeper.Domain.Models;

namespace CandyKeeper.Application.Interfaces
{
    public interface IProductDeliveryService
    {
        Task Create(ProductDelivery productDelivery);
        Task Delete(int id);
        Task<List<ProductDelivery>> Get();
        Task<ProductDelivery> GetById(int id);
        Task Update(ProductDelivery productDelivery);
        Task AddProductForSale(int id, ProductForSale model);
        Task<List<ProductDelivery>> GetBySearchingString(string searchingString);
        Task<List<ProductDelivery>> GetByStoreId(int storeId);
    }
}