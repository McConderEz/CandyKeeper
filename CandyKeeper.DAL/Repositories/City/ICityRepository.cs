using CandyKeeper.Domain.Models;

namespace CandyKeeper.DAL.Repositories
{
    public interface ICityRepository
    {
        Task Create(City city);
        Task Delete(int id);
        Task<List<City>> Get();
        Task<City> GetById(int id);
        Task Update(int id, string name);
    }
}