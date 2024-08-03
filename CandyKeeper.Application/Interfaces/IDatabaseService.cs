using CandyKeeper.Domain.Models;

namespace CandyKeeper.Application.Interfaces;

public interface IDatabaseService
{
    Task<QueryResult> ExecuteQueriesAsync((string description,string value) query,Dictionary<string, object> parameters);
}