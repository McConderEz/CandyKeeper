using CandyKeeper.Domain.Models;

namespace CandyKeeper.DAL.Repositories
{
    public interface IDistrictRepository
    {
        Task Create(District district);
        Task Delete(int id);
        Task<List<District>> Get();
        Task<District> GetById(int id);
        Task Update(int id, string name, int cityId);
    }
}