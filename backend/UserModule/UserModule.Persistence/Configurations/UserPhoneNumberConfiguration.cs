using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SharedKernel.ValueObjects;
using UserModule.Domain.Entities;

namespace UserModule.Persistence.Configurations
{
    public class UserPhoneNumberConfiguration : IEntityTypeConfiguration<UserPhoneNumber>
    {
        public void Configure(EntityTypeBuilder<UserPhoneNumber> builder)
        {
            builder.ToTable("user_phone_numbers")
                .HasKey(upn => upn.Id);

            builder.Property(upn => upn.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id")
                .IsRequired();

            builder.Property(upn => upn.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            var phoneNumberConverter = new ValueConverter<PhoneNumber, string>(
                v => v.Value,
                v => new PhoneNumber(v)
            );

            builder.Property(upn => upn.PhoneNumber)
                .HasColumnName("phone_number")
                .HasConversion(phoneNumberConverter)
                .HasMaxLength(20)
                .IsRequired();
        }
    }
}
