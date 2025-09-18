using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductModule.Domain.Entities;

namespace ProductModule.Persistence.Configurations
{
    public class CharacteristicTemplateConfiguration : IEntityTypeConfiguration<CharacteristicTemplate>
    {
        public void Configure(EntityTypeBuilder<CharacteristicTemplate> builder)
        {
            builder.ToTable("characteristic_templates")
                .HasKey(ct => ct.Id);

            builder.Property(ct => ct.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(ct => ct.Name)
                .HasMaxLength(100)
                .HasColumnName("name")
                .IsRequired();

            builder.Property(ct => ct.Unit)
                .HasMaxLength(50)
                .HasColumnName("unit")
                .IsRequired();

            builder.Property(ct => ct.CategoryId)
                .HasColumnName("category_id")
                .IsRequired();

            builder.Property(ct => ct.Type)
                .HasColumnName("type")
                .HasConversion<string>()
                .IsRequired();

            builder.HasIndex(ct => new { ct.Name, ct.Unit, ct.CategoryId }).IsUnique();
        }
    }
}