using CandyKeeper.Domain.Models;

namespace CandyKeeper.DAL
{
    public interface IStoreRepository
    {
        Task Create(Store store);
        Task Delete(int id);
        Task<List<Store>> Get();
        Task<Store> GetById(int id);
        Task Update(int id, int storeNumber, string name, DateTime yearOfOpened, int numberOfEmployees, string phone, int ownershipTypeId, int districtId);
        Task AddSupplier(int id, Supplier model);
        Task DeleteSupplier(int id, Supplier model);
    }
}