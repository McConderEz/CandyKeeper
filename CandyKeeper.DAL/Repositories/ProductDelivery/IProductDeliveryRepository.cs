using CandyKeeper.Domain.Models;

namespace CandyKeeper.DAL.Repositories
{
    public interface IProductDeliveryRepository
    {
        Task Create(ProductDelivery productDelivery);
        Task Delete(int id);
        Task<List<ProductDelivery>> Get();
        Task<ProductDelivery> GetById(int id);
        Task Update(int id, DateTime deliveryDate, int supplierId, int storeId);
    }
}