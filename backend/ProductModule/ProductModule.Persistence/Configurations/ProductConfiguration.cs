using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProductModule.Domain.Entities;

namespace ProductModule.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products")
                .HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("id");

            builder.Property(p => p.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(p => p.Name)
                .HasColumnName("name")
                .IsRequired();

            builder.OwnsOne(p => p.Price, price =>
            {
                price.Property(p => p.Amount)
                     .HasColumnName("price_amount")
                     .IsRequired();

                price.Property(p => p.Currency)
                     .HasColumnName("price_currency")
                     .IsRequired()
                     .HasMaxLength(10);
            });

            builder.Property(p => p.Location)
                .HasColumnName("location")
                .IsRequired();

            builder.Property(p => p.Description)
                .HasColumnName("description")
                .IsRequired();

            builder.Property(p => p.CategoryId)
                .HasColumnName("category_id")
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.HasMany(p => p.CharacteristicGroups)
               .WithOne()
               .HasForeignKey(p => p.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(p => p.CharacteristicGroups)
               .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}