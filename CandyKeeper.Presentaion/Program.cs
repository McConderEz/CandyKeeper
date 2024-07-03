using CandyKeeper.DAL;
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

namespace CandyKeeper.Presentation
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            var app = new App();
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
                          }, ServiceLifetime.Singleton);

                          services.AddRepositories();
                          services.AddServices();
                          services.AddViewModels();

                      });
            return host;
        }
    }
}
