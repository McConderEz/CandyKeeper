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
    public class ProductForSaleConfiguration : IEntityTypeConfiguration<ProductForSaleEntity>
    {
        public void Configure(EntityTypeBuilder<ProductForSaleEntity> builder)
        {
            builder.HasKey(p => p.Id);
            builder.ToTable(t => t.HasCheckConstraint("VolumeConstraint", "Volume >= 0"))
                .ToTable(t => t.HasCheckConstraint("PriceConstraint", "Price >= 1"));
                

            builder.HasOne(p => p.Store)
                .WithMany(s => s.ProductForSales)
                .HasForeignKey(p => p.StoreId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
