using CandyKeeper.Domain.Models;

namespace CandyKeeper.Application.Interfaces
{
    public interface IDistrictService
    {
        Task Create(District district);
        Task Delete(int id);
        Task<List<District>> Get();
        Task<District> GetById(int id);
        Task Update(District district);
        Task<List<District>> GetBySearchingString(string searchingString);
    }
}