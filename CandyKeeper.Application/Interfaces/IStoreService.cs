using CandyKeeper.Domain.Models;

namespace CandyKeeper.Application.Interfaces
{
    public interface IStoreService
    {
        Task Create(Store store);
        Task Delete(int id);
        Task<List<Store>> Get();
        Task<Store> GetById(int id);
        Task Update(Store store);
    }
}