using CandyKeeper.Domain.Models;

namespace CandyKeeper.DAL
{
    public interface ISupplierRepository
    {
        Task Create(Supplier supplier);
        Task Delete(int id);
        Task<List<Supplier>> Get();
        Task<Supplier> GetById(int id);
        Task Update(int id, string name, string phone, int ownershipTypeId, int cityId);
        Task AddStore(int id, Store model);
        Task DeleteStore(int id, Store model);
    }
}