namespace CandyKeeper.Application.Interfaces;

public interface IPasswordHasherService
{
    string Generate(string password);
    bool Verify(string password, string hashedPassword);
}