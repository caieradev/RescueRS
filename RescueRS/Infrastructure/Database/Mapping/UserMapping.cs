using ResgateRS.Domain.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ResgateRS.Infrastructure.Database.Mapping;
public class UserMapping : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(e => e.UserId);

        builder.Property(e => e.UserId);
        builder.Property(e => e.Rescuer)
            .HasConversion<byte>();
        builder.Property(e => e.Cellphone);
    }
}