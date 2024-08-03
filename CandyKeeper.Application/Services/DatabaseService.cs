using System.Data;
using CandyKeeper.Application.Interfaces;
using Dapper;
using CandyKeeper.Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CandyKeeper.Application.Services;


public class DatabaseService : IDatabaseService
{
    private readonly IConfiguration _configuration;

    public DatabaseService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<QueryResult> ExecuteQueriesAsync((string description, string value) query, Dictionary<string, object> parameters)
    {
        var result = new QueryResult();

        await using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync();

            await using (var command = new SqlCommand(query.value, connection))
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }

                await using (var reader = await command.ExecuteReaderAsync())
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);

                    result.Description = query.description;
                    result.QueryName = query.value;
                    result.Result = dataTable;
                }
            }
        }

        return result;
    }
}