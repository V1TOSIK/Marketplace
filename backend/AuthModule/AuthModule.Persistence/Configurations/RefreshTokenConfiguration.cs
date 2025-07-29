using AuthModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthModule.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<Domain.Entities.RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_tokens");

            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Id)
                .HasColumnName("id");

            builder.HasOne<AuthUser>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(rt => rt.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(rt => rt.Token)
                .HasColumnName("token")
                .IsRequired()
                .HasMaxLength(500);
            
            builder.Property(rt => rt.ExpirationDate)
                .HasColumnName("expiration_date")
                .IsRequired();
            
            builder.Property(rt => rt.IsRevoked)
                .HasColumnName("is_revoked")
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(rt => rt.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();
            
            builder.Property(rt => rt.RevokedAt)
                .HasColumnName("revoked_at")
                .IsRequired(false);

            builder.Property(rt => rt.ReplacedByTokenId)
                .HasColumnName("replaced_by_token_id")
                .IsRequired(false);

            builder.Property(rt => rt.Device)
                .HasColumnName("device")
                .IsRequired();

            builder.Property(rt => rt.IpAddress)
                .HasColumnName("ip_address")
                .IsRequired();

            builder.HasIndex(rt => rt.UserId);
        }
    }
}
