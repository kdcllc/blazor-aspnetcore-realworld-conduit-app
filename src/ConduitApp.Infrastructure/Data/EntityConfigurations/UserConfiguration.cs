using ConduitApp.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConduitApp.Infrastructure.Data.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "user");

        builder.Property(e => e.Id)
        .UseIdentityColumn()
        .ValueGeneratedOnAdd();

        builder.HasKey(k => k.Id).IsClustered();

        builder.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(true)
                .HasColumnName("Email");

        builder.Property(e => e.UserName)
                .HasMaxLength(255)
                .IsUnicode(true)
                .HasColumnName("UserName");
    }
}
