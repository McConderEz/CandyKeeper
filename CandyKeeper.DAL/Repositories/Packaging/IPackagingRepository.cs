using CandyKeeper.Domain.Models;

namespace CandyKeeper.DAL.Repositories
{
    public interface IPackagingRepository
    {
        Task Create(Packaging packaging);
        Task Delete(int id);
        Task<List<Packaging>> Get();
        Task<Packaging> GetById(int id);
        Task Update(int id, string name);
    }
}