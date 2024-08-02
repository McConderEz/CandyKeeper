using CandyKeeper.DAL;
using CandyKeeper.Presentation;
using CandyKeeper.Presentation.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CandyKeeper.Application.Interfaces;
using CandyKeeper.Application.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CandyKeeper.Presentation
{
    public class Program
    {
        private static string[] roleNames = { "Admin", "Manager", "Client" };
        
        [STAThread]
        public static void Main()
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] Args)
        {
            var host = Host.CreateDefaultBuilder(Args)
                      .ConfigureServices((context, services) =>
                      {
                          services.AddDbContext<CandyKeeperDbContext>(options =>
                          {
                              options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection"));
                          }, ServiceLifetime.Scoped);
                          services.AddDbContext<AuthCandyKeeperDbContext>(options =>
                          {
                              options.UseSqlServer(context.Configuration.GetConnectionString("AuthConnection"));
                          }, ServiceLifetime.Scoped); 
                          services.AddMemoryCache();
                          services.AddRepositories();
                          services.AddServices();
                          services.AddViewModels();

                      })
                      .ConfigureServices((context, services) =>
                      {
                          var serviceProvider = services.BuildServiceProvider();
                          serviceProvider.EnsureDatabaseMigrated<CandyKeeperDbContext>();
                          serviceProvider.EnsureDatabaseMigrated<AuthCandyKeeperDbContext>();

                          var authConnectionString = context.Configuration.GetConnectionString("AuthConnection")!;
                          services.EnsureRolesExist(authConnectionString, roleNames);
                      });
            
            return host;
        }
    }
}
