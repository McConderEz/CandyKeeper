namespace CandyKeeper.DAL.Repositories.User;

public interface IUserRepository
{
    Task Create(Domain.Models.User user);
    Task Delete(int id);
    Task<List<Domain.Models.User>> Get();
    Task<Domain.Models.User> GetById(int id);
    Task Update(int id, string name,int principalId, int? storeId);
    Task<Domain.Models.User> GetByUserName(string userName);
}