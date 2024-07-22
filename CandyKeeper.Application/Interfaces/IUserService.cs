namespace CandyKeeper.Application.Interfaces;

public interface IUserService
{
    Task Create(Domain.Models.User user);
    Task Delete(int id);
    Task<List<Domain.Models.User>> Get();
    Task<Domain.Models.User> GetById(int id);
    Task Update(int id, string name,int principalId, int? storeId);
}