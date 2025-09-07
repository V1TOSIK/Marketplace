using AuthModule.Domain.Entities;
using AuthModule.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SharedKernel.ValueObjects;

namespace AuthModule.Persistence.Configurations
{
    public class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
    {
        public void Configure(EntityTypeBuilder<AuthUser> builder)
        {
            builder.ToTable("auth_users")
                .HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasColumnName("id");

            var emailConverter = new ValueConverter<Email?, string>(
                v => v == null ? null : v.Value,
                v => string.IsNullOrWhiteSpace(v) ? null : new Email(v)
            );

            var phoneNumberConverter = new ValueConverter<PhoneNumber?, string>(
                v => v == null ? null : v.Value,
                v => string.IsNullOrWhiteSpace(v) ? null : new PhoneNumber(v)
            );

            var passwordConverter = new ValueConverter<Password?, string>(
                v => v == null ? null : v.Value,
                v => string.IsNullOrWhiteSpace(v) ? null : new Password(v)
            );

            builder.Property(u => u.ProviderUserId)
                .HasColumnName("provider_user_id")
                .HasMaxLength(255)
                .IsRequired(false);

            builder.Property(u => u.Email)
                .HasColumnName("email")
                .HasConversion(emailConverter)
                .HasMaxLength(255)
                .IsRequired(false);

            builder.Property(u => u.PhoneNumber)
                .HasColumnName("phone_number")
                .HasConversion(phoneNumberConverter)
                .HasMaxLength(20)
                .IsRequired(false);

            builder.Property(u => u.Password)
                .HasColumnName("password")
                .HasConversion(passwordConverter)
                .HasMaxLength(255)
                .IsRequired(false);

            builder.Property(u => u.Provider)
                .HasColumnName("provider")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(u => u.Role)
                .HasColumnName("role")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(u => u.RegistrationDate)
                .HasColumnName("registration_date")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(u => u.IsBanned)
                .HasColumnName("is_banned")
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(u => u.BanReason)
                .HasColumnName("ban_reason")
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(u => u.BannedAt)
                .HasColumnName("banned_at")
                .IsRequired(false);

            builder.Property(u => u.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(u => u.DeletedAt)
                .HasColumnName("deleted_at")
                .IsRequired(false);

            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.PhoneNumber).IsUnique();
        }
    }
}
