using CandyKeeper.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CandyKeeper.DAL
{
    public class CandyKeeperDbContext : DbContext
    {
        //TODO: Сделать запросы по 5 и 6 лабе в коде
        //TODO: Сделать диаграмму и вывод в эксель
        
        //TODO: Триггеры
        
        //TODO: Пофиксить проблему сброса роли, после смена магазина
        //TODO: Протестировать смену роли и отображение нужных вкладок
        
        private readonly IConfiguration _configuration;
        
        public CandyKeeperDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            Database.EnsureCreated();
        }
        
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }
        
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CandyKeeperDbContext).Assembly);
        }

        public DbSet<CityEntity> Cities { get; set; } = null!;
        public DbSet<DistrictEntity> Districts { get; set; } = null!;
        public DbSet<OwnershipTypeEntity> OwnershipTypes { get; set; } = null!;
        public DbSet<PackagingEntity> Packaging { get; set; } = null!;
        public DbSet<ProductDeliveryEntity> ProductDeliveries { get; set; } = null!;
        public DbSet<ProductEntity> Products { get; set; } = null!;
        public DbSet<ProductForSaleEntity> ProductForSales { get; set;} = null!;
        public DbSet<ProductTypeEntity> ProductTypes { get; set; } = null!;
        public DbSet<StoreEntity> Stores { get; set; } = null!;
        public DbSet<SupplierEntity> Suppliers { get; set; } = null!;
        public DbSet<UserEntity> Users { get; set; } = null!;
    }
}
