using CandyKeeper.Domain.Models;

namespace CandyKeeper.Application.Interfaces
{
    public interface ICityService
    {
        Task Create(City city);
        Task Delete(int id);
        Task<List<City>> Get();
        Task<City> GetById(int id);
        Task Update(City city);
        Task<List<City>> GetBySearchingString(string searchingString);
    }
}