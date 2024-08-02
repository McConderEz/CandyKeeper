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

    public async Task<QueryResult> ExecuteQueriesAsync((string description,string value) query)
    {
        var results = new List<QueryResult>();

        await using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync();
            
            await using (var reader = await connection.ExecuteReaderAsync(query.value))
            {
                var dataTable = new DataTable();
                dataTable.Load(reader);

                results.Add(new QueryResult
                {
                    Description = query.description,
                    QueryName = query.value,
                    Result = dataTable
                });
            }
            
        }

        return results.FirstOrDefault();
    }
}