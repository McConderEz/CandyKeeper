using CandyKeeper.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.DAL
{
    public class TestDbContext: DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase("TestDatabase");
            }
        }

        public DbSet<CityEntity> Cities { get; set; } = null!;
        public DbSet<DistrictEntity> Districts { get; set; } = null!;
        public DbSet<OwnershipTypeEntity> OwnershipTypes { get; set; } = null!;
        public DbSet<PackagingEntity> Packaging { get; set; } = null!;
        public DbSet<ProductDeliveryEntity> ProductDeliveries { get; set; } = null!;
        public DbSet<ProductEntity> Products { get; set; } = null!;
        public DbSet<ProductForSaleEntity> ProductForSales { get; set; } = null!;
        public DbSet<ProductTypeEntity> ProductTypes { get; set; } = null!;
        public DbSet<StoreEntity> Stores { get; set; } = null!;
        public DbSet<SupplierEntity> Suppliers { get; set; } = null!;
    }
}
