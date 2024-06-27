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
    public class OwnershipTypeConfiguration : IEntityTypeConfiguration<OwnershipTypeEntity>
    {
        public void Configure(EntityTypeBuilder<OwnershipTypeEntity> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Name)
                .HasMaxLength(OwnershipType.MAX_NAME_SIZE)
                .IsRequired();

            builder.HasMany(o => o.Stores)
                .WithOne(s => s.OwnershipType)
                .HasForeignKey(s => s.OwnershipTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
