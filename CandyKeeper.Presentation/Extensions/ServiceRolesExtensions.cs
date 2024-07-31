using System.Diagnostics;
using CandyKeeper.Application.Interfaces;
using CandyKeeper.DAL;
using MaterialDesignColors;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace CandyKeeper.Presentation.Extensions;

public static class ServiceRolesExtensions
{
    private static string[] _adminTables =
    {
        "Cities",
        "Districts",
        "OwnershipTypes",
        "Packaging",
        "ProductTypes"
    };
    
    private static string[] _managerTables =
    {
        "ProductDeliveries",
        "ProductForSales",
        "Products",
        "Stores",
        "Suppliers"
    };
    
    private static string[] _clientTables =
    {
        "ProductForSales",
    };

    private static string[] _permissions = { "SELECT", "INSERT", "UPDATE", "DELETE" };
    
    
    public static bool DoesRoleExist(string connectionString, string roleName)
        {
            string checkRoleSql = $"""
                                   
                                                   SELECT COUNT(*) 
                                                   FROM sys.database_principals
                                                   WHERE type = 'R' AND name = @roleName
                                   """;


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(checkRoleSql, connection))
                    {
                        command.Parameters.AddWithValue("@roleName", roleName);
                        int roleCount = (int)command.ExecuteScalar();
                        return roleCount > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return false;
                }
            }
        }

        public static void CreateRole(string connectionString, string roleName)
        {
            string createRoleSql = $"""
                                    
                                                        CREATE ROLE {roleName};
                                    """;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(createRoleSql, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine($"Role '{roleName}' created successfully.");
                    }

                    string getPrincipalIdSql = $"""
                                                
                                                                             SELECT principal_id 
                                                                             FROM sys.database_principals 
                                                                             WHERE name = @roleName
                                                """;
                    using(SqlCommand command = new SqlCommand(getPrincipalIdSql, connection))
                    {
                        command.Parameters.AddWithValue("@roleName", roleName);
                        var principalId = (int)command.ExecuteScalar();
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        public static void EnsureRolesExist(this IServiceCollection services,string connectionString, string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                if (!DoesRoleExist(connectionString, roleName))
                {
                    CreateRole(connectionString, roleName);
                    GrantPermissionOnTable(connectionString, roleName);
                }
                else
                {
                    Console.WriteLine($"Role '{roleName}' already exists.");
                }
            }
        }


        private static void GrantPermissionOnTable(string connectionString,string roleName)
        {
            switch(roleName)
            {
                case "Admin":
                    GrantPermission(connectionString, roleName,_adminTables, string.Join(",",_permissions).TrimEnd(','));
                    break;
                case "Manager": 
                    GrantPermission(connectionString, roleName,_managerTables, string.Join(",",_permissions).TrimEnd(','));
                    break;
                case "Client":
                    GrantPermission(connectionString, roleName, _clientTables, _permissions[0]);
                    break;
                default: throw new ArgumentException($"{roleName} is not exist");
            }
        }
        

        private static void GrantPermission(string connectionString,string roleName ,string[] tables, string permissions)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    for (var i = 0; i < tables.Length; i++)
                    {
                        string grantPermToRoleSql = $"""
                                                     
                                                                         GRANT {permissions} ON 
                                                                         {tables[i]}
                                                                         TO {roleName};
                                                     """;
                        using (SqlCommand command = new SqlCommand(grantPermToRoleSql, connection))
                        {
                            command.ExecuteNonQuery();
                            Console.WriteLine($"Role got permissions successfully.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        
}