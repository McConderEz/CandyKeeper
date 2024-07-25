using CandyKeeper.Domain.Models;

namespace CandyKeeper.Application.Interfaces;

public interface IAccountService
{
    Task Register(string userName, string password);
    Task AssignRoleToUser(string connectionString, string userName, string roleName);
    Task<User> Login(string userName, string password);
    Task AddRoot();
    Task DropRoleToUser(string connectionString, string userName, string roleName);
}