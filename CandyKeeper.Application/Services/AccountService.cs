using CandyKeeper.Application.Interfaces;
using CandyKeeper.Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

namespace CandyKeeper.Application.Services;


public class AccountService : IAccountService
{
    private readonly IUserService _userService;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IConfiguration _configuration;

    public AccountService(IUserService userService, IPasswordHasherService passwordHasherService,IConfiguration configuration)
    {
        _userService = userService;
        _passwordHasherService = passwordHasherService;
        _configuration = configuration;
    }
    
    public async Task Register(string userName, string password)
    {
        var hashedPassword = _passwordHasherService.Generate(password);
        var user = await _userService.GetByUserName(userName);

        string createUserSql = $"""
                                
                                                             CREATE LOGIN {userName} WITH PASSWORD = '{hashedPassword}';
                                                             CREATE USER {userName} FOR LOGIN {userName};
                                                                
                                """;
        if (user == null)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(createUserSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    
                    string getPrincipalIdSql = $"""
                                                
                                                                                                  SELECT principal_id 
                                                                                                  FROM sys.database_principals 
                                                                                                  WHERE name = 'Client'
                                                """;

                    AssignRoleToUser(_configuration.GetConnectionString("DefaultConnection"), userName, "Client");

                    using (SqlCommand command = new SqlCommand(getPrincipalIdSql, connection))
                    {
                        command.Parameters.AddWithValue("roleName", "Client");
                        var principalId = (int)command.ExecuteScalar();
                        user = User.Create(0, userName, hashedPassword, principalId, 8).Value;
                    
                        await _userService.Create(user);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        else
        {
            throw new Exception("User already exist");
        }
    }
    
    public void AssignRoleToUser(string connectionString, string userName, string roleName)
    {
        string assignRoleSql = $"""
                                
                                                         ALTER ROLE [{roleName}] ADD MEMBER [{userName}];
                                                                                                        
                                """;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(assignRoleSql, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine($"User '{userName}' assigned to role '{roleName}' successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
    
    public void DropRoleToUser(string connectionString, string userName, string roleName)
    {
        string assignRoleSql = $"""
                                 
                                                          ALTER ROLE [{roleName}] DROP MEMBER [{userName}];
                                                                                                         
                                 """;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(assignRoleSql, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine($"User '{userName}' drop from role '{roleName}' successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
    
    public async Task<User> Login(string userName, string password)
    {
        var user = await _userService.GetByUserName(userName);

        if(user == null)
        {
            return null;
        }

        var result = _passwordHasherService.Verify(password, user.PasswordHashed);

        if(result == false)
        {
            return null;
        }

        return user;
    }

    public async Task AddRoot()
    {
        var hashedPassword = _passwordHasherService.Generate("Root");
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"))) 
        {
            try
                {
                    connection.Open();

                    string checkUserSql = $"""
                                           
                                                           IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'Root')
                                                           BEGIN
                                                               CREATE LOGIN [Root] WITH PASSWORD = '{hashedPassword}';
                                                           END;
                                           
                                                           IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'Root')
                                                           BEGIN
                                                               CREATE USER [Root] FOR LOGIN [Root];
                                                           END;
                                           """;

                    using (SqlCommand command = new SqlCommand(checkUserSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    string getPrincipalIdSql = """
                                               
                                                               SELECT principal_id 
                                                               FROM sys.database_principals 
                                                               WHERE name = 'Admin'
                                               """;


                    AssignRoleToUser(_configuration.GetConnectionString("DefaultConnection"), "Root", "Admin");

                    using (SqlCommand command = new SqlCommand(getPrincipalIdSql, connection))
                    {
                        var principalId = (int)command.ExecuteScalar();

                        User user = await _userService.GetByUserName("Root");
                        
                        if (user != null)
                            return;
                        
                        user = User.Create(0, "Root", hashedPassword, principalId).Value;
                        await _userService.Create(user);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
        }
    }

    
}