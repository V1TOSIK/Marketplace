using AuthUserModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthUserModule.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<Domain.Entities.RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne<AuthUser>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);
            
            builder.Property(rt => rt.ExpirationDate)
                .IsRequired();
            
            builder.Property(rt => rt.IsRevoked)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(rt => rt.CreatedAt)
                .IsRequired();
            
            builder.Property(rt => rt.RevokedAt)
                .IsRequired(false);
        }
    }
}
