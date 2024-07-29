using CandyKeeper.DAL.Entities;
using CandyKeeper.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.DAL.Configuration
{
    public class StoreConfiguration : IEntityTypeConfiguration<StoreEntity>
    {
        public void Configure(EntityTypeBuilder<StoreEntity> builder)
        {
            builder.HasKey(s => s.Id);

            builder
                .ToTable(t =>
                    t.HasCheckConstraint("YearOfOpenedConstraint", $"YEAR(YearOfOpened) <= YEAR(GETDATE())"))
                .ToTable(t =>
                    t.HasCheckConstraint("StoreNumberConstrains", "StoreNumber >= 100000 And StoreNumber <= 999999"));

            builder.Property(s => s.Name)
                .HasMaxLength(Store.MAX_NAME_SIZE);

            builder.HasMany(s => s.Suppliers)
            .WithMany(s => s.Stores)
            .UsingEntity<Dictionary<string, object>>(
                "StoreSupplierLink",
                j => j
                    .HasOne<SupplierEntity>()
                    .WithMany()
                    .HasForeignKey("SuppliersId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<StoreEntity>()
                    .WithMany()
                    .HasForeignKey("StoresId")
                    .OnDelete(DeleteBehavior.Restrict));

        }
    }
}
