using CandyKeeper.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.VisualBasic.ApplicationServices;
using User = CandyKeeper.Domain.Models.User;

namespace CandyKeeper.DAL.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .HasMaxLength(User.MAX_NAME_SIZE)
            .IsRequired();

        builder.HasOne(u => u.Store)
            .WithMany()
            .HasForeignKey(u => u.StoreId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}