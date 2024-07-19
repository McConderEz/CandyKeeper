using CandyKeeper.Domain.Models;

namespace CandyKeeper.Application.Interfaces
{
    public interface ISupplierService
    {
        Task Create(Supplier supplier);
        Task Delete(int id);
        Task<List<Supplier>> Get();
        Task<Supplier> GetById(int id);
        Task Update(Supplier supplier);
        Task AddStore(int id, Store model);
        Task DeleteStore(int id, Store model);
    }
}