using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserModule.Domain.Entities;

namespace UserModule.Persistence.Configurations
{
    public class UserBlockConfiguration : IEntityTypeConfiguration<UserBlock>
    {
        public void Configure(EntityTypeBuilder<UserBlock> builder)
        {
            builder.ToTable("user_blocks")
                .HasKey(ub => ub.Id);

            builder.Property(ub => ub.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id")
                .IsRequired();

            builder.Property(ub => ub.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(ub => ub.BlockedUserId)
                .HasColumnName("blocked_user_id")
                .IsRequired();

            builder.Property(ub => ub.BlockedAt)
                .HasColumnName("blocked_at")
                .IsRequired();

            builder.Property(ub => ub.UnblockedAt)
                .HasColumnName("unblocked_at")
                .IsRequired(false);

            builder.HasIndex(ub => new { ub.UserId, ub.BlockedUserId }).IsUnique();

        }
    }
}
