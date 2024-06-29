using CandyKeeper.Domain.Models;

namespace CandyKeeper.DAL
{
    public interface IProductForSaleRepository
    {
        Task Create(ProductForSale productForSale);
        Task Delete(int id);
        Task<List<ProductForSale>> Get();
        Task<ProductForSale> GetById(int id);
        Task Update(int id, decimal price, int volume, int productId, int storeId, int productDeliveryId, int packagingId);
    }
}