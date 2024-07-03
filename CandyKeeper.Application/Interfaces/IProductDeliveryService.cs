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
    }
}