using CandyKeeper.DAL.Entities;
using CandyKeeper.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CandyKeeper.DAL.Configuration;

public class CityConfiguration : IEntityTypeConfiguration<CityEntity>
{
    public void Configure(EntityTypeBuilder<CityEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(d => d.Name)
            .HasMaxLength(City.MAX_NAME_LENGTH)
            .IsRequired();

        builder.HasMany(c => c.Suppliers)
            .WithOne(s => s.City)
            .HasForeignKey(s => s.CityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}