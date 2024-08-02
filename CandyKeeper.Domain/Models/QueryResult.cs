using System.Data;

namespace CandyKeeper.Domain.Models;

public class QueryResult
{
    public string Description { get; set; }
    public string QueryName { get; set; }
    public DataTable Result { get; set; }
}