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
    public class PackagingConfiguration : IEntityTypeConfiguration<PackagingEntity>
    {
        public void Configure(EntityTypeBuilder<PackagingEntity> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .HasMaxLength(Packaging.MAX_NAME_SIZE)
                .IsRequired();

            builder.HasMany(p => p.Products)
                .WithOne(p => p.Packaging)
                .HasForeignKey(p => p.PackagingId)
                .OnDelete(DeleteBehavior.Cascade);  
        }
    }
}
