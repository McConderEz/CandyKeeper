using CandyKeeper.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.DAL
{
    public class CandyKeeperDbContext : DbContext
    {
        //TODO: Добавить Constraints
        //TODO: Добавить UserEntity
        //TODO:Сделать регистрацию и авторизацию

        //TODO: Спроектировать интерфейс и CRUD в нём

        public CandyKeeperDbContext(DbContextOptions<CandyKeeperDbContext> options) 
            : base(options)
        {
            Database.EnsureCreated();
        }

        public CandyKeeperDbContext()
        {
            Database.EnsureCreated();
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
    }
}
