using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ProductModule.Domain.Entities;

public class CharacteristicGroupConfiguration : IEntityTypeConfiguration<CharacteristicGroup>
{
    public void Configure(EntityTypeBuilder<CharacteristicGroup> builder)
    {
        builder.ToTable("characteristic_groups")
            .HasKey(cg => cg.Id);

        builder.Property(cg => cg.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(cg => cg.Name)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(cg => cg.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.HasMany(cg => cg.CharacteristicValues)
            .WithOne()
            .HasForeignKey(cv => cv.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(cg => cg.CharacteristicValues)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(cg => cg.ProductId);
    }
}
