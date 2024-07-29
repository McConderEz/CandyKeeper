using CandyKeeper.Domain.Models;

namespace CandyKeeper.Application.Interfaces
{
    public interface IOwnershipTypeService
    {
        Task Create(OwnershipType ownershipType);
        Task Delete(int id);
        Task<List<OwnershipType>> Get();
        Task<OwnershipType> GetById(int id);
        Task Update(OwnershipType ownershipType);
        Task<List<OwnershipType>> GetBySearchingString(string searchingString);
    }
}