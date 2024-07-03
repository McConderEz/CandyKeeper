using CandyKeeper.Application.Interfaces;
using CandyKeeper.Application.Services;
using CandyKeeper.DAL;
using CandyKeeper.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.Presentation.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IDistrictRepository, DistrictRepository>();
            services.AddScoped<IPackagingRepository, PackagingRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductDeliveryRepository, ProductDeliveryRepository>();
            services.AddScoped<IProductForSaleRepository, IProductForSaleRepository>();
            services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IDistrictService, DistrictService>();
            services.AddScoped<IPackagingService, PackagingService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductDeliveryService, ProductDeliveryService>();
            services.AddScoped<IProductForSaleService, IProductForSaleService>();
            services.AddScoped<IProductTypeService, ProductTypeService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<ISupplierService, SupplierService>();

            return services;
        }
    }
}
