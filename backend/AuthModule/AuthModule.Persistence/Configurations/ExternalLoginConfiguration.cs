using AuthModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthModule.Persistence.Configurations
{
    public class ExternalLoginConfiguration : IEntityTypeConfiguration<ExternalLogin>
    {
        public void Configure(EntityTypeBuilder<ExternalLogin> builder)
        {
            builder.ToTable("ExternalLogins")
                .HasKey(el => new { el.UserId, el.Provider });

            builder.Property(el => el.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(el => el.Provider)
                .HasColumnName("provider")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(el => el.ProviderUserId)
                .HasColumnName("provider_user_id")
                .IsRequired();
        }
    }
}
