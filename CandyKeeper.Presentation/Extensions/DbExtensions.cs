using Microsoft.Data.SqlClient;

namespace CandyKeeper.Presentation.Extensions;

public static class DbExtensions
{
    public static string CreateConnectionString(string server, string database, string userId, string password)
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = server,
            InitialCatalog = database,
            UserID = userId,
            Password = password,
            IntegratedSecurity = false 
        };
        return builder.ConnectionString;
    }
}