using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductModule.Domain.Entities;

namespace ProductModule.Persistence.Configurations
{
    public class CharacteristicValueConfiguration : IEntityTypeConfiguration<CharacteristicValue>
    {
        public void Configure(EntityTypeBuilder<CharacteristicValue> builder)
        {
            builder.ToTable("characteristic_values")
                .HasKey(cv => new { cv.Value, cv.CharacteristicTemplateId, cv.GroupId});

            builder.Property(cv => cv.Value)
                .HasColumnName("value")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(cv => cv.CharacteristicTemplateId)
                .HasColumnName("characteristic_template_id")
                .IsRequired();

            builder.Property(cv => cv.GroupId)
                .HasColumnName("group_id")
                .IsRequired();
        }
    }
}
