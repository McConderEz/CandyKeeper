using CandyKeeper.Application.Interfaces;
using CandyKeeper.Application.Services;
using CandyKeeper.DAL;
using CandyKeeper.DAL.Repositories;
using CandyKeeper.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CandyKeeper.DAL.Repositories.User;
using CandyKeeper.Presentation.ViewModels.Base;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.ApplicationServices;

namespace CandyKeeper.Presentation.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IDistrictRepository, DistrictRepository>();
            services.AddScoped<IOwnershipTypeRepository, OwnershipTypeRepository>();
            services.AddScoped<IPackagingRepository, PackagingRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductDeliveryRepository, ProductDeliveryRepository>();
            services.AddScoped<IProductForSaleRepository, ProductForSaleRepository>();
            services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IDistrictService, DistrictService>();
            services.AddScoped<IOwnershipTypeService, OwnershipTypeService>();
            services.AddScoped<IPackagingService, PackagingService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductDeliveryService, ProductDeliveryService>();
            services.AddScoped<IProductForSaleService, ProductForSaleService>();
            services.AddScoped<IProductTypeService, ProductTypeService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();

            services.AddSingleton<IUserSessionService, UserSessionService>();
            
            return services;
        }

        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services.AddSingleton<ViewModelLocator>();
            services.AddSingleton<MainWindowsViewModel>();
            services.AddSingleton<CityViewModel>();
            services.AddSingleton<DistrictViewModel>();
            services.AddSingleton<OwnershipTypeViewModel>();
            services.AddSingleton<PackagingViewModel>();
            services.AddSingleton<ProductDeliveryViewModel>();
            services.AddSingleton<ProductForSaleViewModel>();
            services.AddSingleton<ProductTypeViewModel>();
            services.AddSingleton<ProductViewModel>();
            services.AddSingleton<StoreViewModel>();
            services.AddSingleton<SupplierViewModel>();
            services.AddSingleton<UserViewModel>();

            return services;
        }
        
        public static void EnsureDatabaseMigrated<T>(this IServiceProvider serviceProvider) where T : DbContext
        {
            try
            {
                var context = serviceProvider.GetRequiredService<T>();
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }
}
