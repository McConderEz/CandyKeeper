using CandyKeeper.Application.Interfaces;

namespace CandyKeeper.Application.Services;

public interface PasswordHasherService: IPasswordHasherService
{
    public string Generate(string password) =>
        BCrypt.Net.BCrypt.EnhancedHashPassword(password);

    public bool Verify(string password, string hashedPassword) =>
        BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
}