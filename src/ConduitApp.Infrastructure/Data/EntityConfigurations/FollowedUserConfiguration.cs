using ConduitApp.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConduitApp.Infrastructure.Data.EntityConfigurations;

public class FollowedUserConfiguration : IEntityTypeConfiguration<FollowedUser>
{
    public void Configure(EntityTypeBuilder<FollowedUser> builder)
    {
        builder.ToTable("FollowedUsers", "user");

        builder.HasKey(t => new { t.UserId, t.FollowedUserId });

        // we need to add OnDelete RESTRICT otherwise for the SqlServer database provider,
        // app.ApplicationServices.GetRequiredService<ConduitContext>().Database.EnsureCreated(); throws the following error:
        // System.Data.SqlClient.SqlException
        // HResult = 0x80131904
        // Message = Introducing FOREIGN KEY constraint 'FK_FollowedPeople_Persons_TargetId' on table 'FollowedPeople' may cause cycles or multiple cascade paths.Specify ON DELETE NO ACTION or ON UPDATE NO ACTION, or modify other FOREIGN KEY constraints.
        // Could not create constraint or index. See previous errors.
        builder.HasOne(pt => pt.User)
            .WithMany(p => p!.FollowedUsers)
            .HasForeignKey(pt => pt.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // we need to add OnDelete RESTRICT otherwise for the SqlServer database provider,
        // app.ApplicationServices.GetRequiredService<ConduitContext>().Database.EnsureCreated(); throws the following error:
        // System.Data.SqlClient.SqlException
        // HResult = 0x80131904
        // Message = Introducing FOREIGN KEY constraint 'FK_FollowingPeople_Persons_TargetId' on table 'FollowedPeople' may cause cycles or multiple cascade paths.Specify ON DELETE NO ACTION or ON UPDATE NO ACTION, or modify other FOREIGN KEY constraints.
        // Could not create constraint or index. See previous errors.
        builder.HasOne(pt => pt.FollowerUser)
            .WithMany(t => t!.Followers)
            .HasForeignKey(pt => pt.FollowedUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
