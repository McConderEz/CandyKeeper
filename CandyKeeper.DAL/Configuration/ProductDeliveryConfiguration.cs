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
    public class ProductDeliveryConfiguration : IEntityTypeConfiguration<ProductDeliveryEntity>
    {
        public void Configure(EntityTypeBuilder<ProductDeliveryEntity> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasMany(p => p.ProductForSales)
                .WithOne(p => p.ProductDelivery)
                .HasForeignKey(p => p.ProductDeliveryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Supplier)
                .WithMany(s => s.ProductDeliveries)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Store)
                .WithMany(s => s.ProductDeliveries)
                .HasForeignKey(p => p.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
