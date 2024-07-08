using CandyKeeper.Domain.Models;

namespace CandyKeeper.Application.Services
{
    public interface IPackagingService
    {
        Task Create(Packaging packaging);
        Task Delete(int id);
        Task<List<Packaging>> Get();
        Task<Packaging> GetById(int id);
        Task Update(Packaging packaging);
    }
}