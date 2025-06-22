using AuthUserModule.Domain.Entities;
using AuthUserModule.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuthUserModule.Persistence.Configurations
{
    class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
    {
        public void Configure(EntityTypeBuilder<AuthUser> builder)
        {
            builder.ToTable("auth_users")
                .HasKey(u => u.UserId);

            builder.Property(u => u.UserId)
                .HasColumnName("user_id");

            var emailConverter = new ValueConverter<Email?, string>(
                v => v == null ? null : v.Value,
                v => string.IsNullOrWhiteSpace(v) ? null : new Email(v)
            );

            var phoneNumberConverter = new ValueConverter<PhoneNumber?, string>(
                v => v == null ? null : v.Value,
                v => string.IsNullOrWhiteSpace(v) ? null : new PhoneNumber(v)
            );

            var passwordConverter = new ValueConverter<Password, string>(
                v => v.Value,
                v => new Password(v)
            );

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
                .IsRequired();

            builder.Property(u => u.Role)
                .HasColumnName("role")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(u => u.RegistrationDate)
                .HasColumnName("registration_date")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();
        }
    }
}
