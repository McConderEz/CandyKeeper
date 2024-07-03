using CandyKeeper.DAL;
using CandyKeeper.Presentation.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Forms;

namespace CandyKeeper.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public App()
        {
            var host = Host.CreateDefaultBuilder()
                      .ConfigureServices((context, services) =>
                      {
                          services.AddDbContext<CandyKeeperDbContext>(options =>
                          {
                              options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection"));
                          }, ServiceLifetime.Singleton);

                          services.AddRepositories();
                          services.AddServices();

                      })
                      .Build();
        }
    }

}
