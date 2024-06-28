using CandyKeeper.Domain.Models;

namespace CandyKeeper.DAL.Repositories
{
    public interface IOwnershipTypeRepository
    {
        Task Create(OwnershipType ownershipType);
        Task Delete(int id);
        Task<List<OwnershipType>> Get();
        Task<OwnershipType> GetById(int id);
        Task Update(int id, string name);
    }
}