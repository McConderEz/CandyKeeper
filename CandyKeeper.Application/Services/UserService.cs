using CandyKeeper.Application.Interfaces;
using CandyKeeper.DAL.Repositories.User;
using CandyKeeper.Domain.Models;

namespace CandyKeeper.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public async Task Create(User user)
    {
        await _repository.Create(user);
    }

    public async Task Delete(int id)
    {
        await _repository.Delete(id);
    }

    public async Task<List<User>> Get()
    {
        return await _repository.Get();
    }

    public async Task<User> GetById(int id)
    {
        return await _repository.GetById(id);
    }

    public async Task<User> GetByUserName(string userName)
    {
        return await _repository.GetByUserName(userName);
    }

    public async Task Update(int id, string name,int principalId, int? storeId, bool isBlocked)
    {
        await _repository.Update(id, name, principalId ,storeId, isBlocked);
    }
    
}