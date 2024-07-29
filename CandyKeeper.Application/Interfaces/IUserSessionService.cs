namespace CandyKeeper.Application.Interfaces;

public interface IUserSessionService
{
    void SetUserData(string key, object value, TimeSpan? absoluteExpirationRelativeToNow = null);
    T GetUserData<T>(string key);
}