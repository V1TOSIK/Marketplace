using MediaModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaModule.Persistence.Configurations
{
    public class MediaConfiguration : IEntityTypeConfiguration<Media>
    {
        public void Configure(EntityTypeBuilder<Media> builder)
        {
            builder.ToTable("medias")
                .HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .HasColumnName("id");

            builder.Property(m => m.EntityId)
                .HasColumnName("entity_id")
                .IsRequired();

            builder.Property(m => m.Url)
                .HasColumnName("url")
                .IsRequired();

            builder.Property(m => m.Name)
                .HasColumnName("name")
                .IsRequired();

            builder.Property(m => m.IsMain)
                .HasColumnName("is_main")
                .HasDefaultValue(false);

            builder.Property(m => m.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);

            builder.Property(m => m.MediaType)
                .HasColumnName("media_type")
                .IsRequired();

            builder.Property(m => m.EntityType)
                .HasConversion<string>()
                .HasColumnName("entity_type")
                .IsRequired();

            builder.Property(m => m.DeletedAt)
                .HasColumnName("deleted_at");

            builder.Property(m => m.CreatedAt)
                .HasColumnName("created_at");

            builder.HasIndex(m => m.EntityId)
                .HasDatabaseName("IX_Media_EntityId");
            builder.HasIndex(m => m.IsMain)
                .HasDatabaseName("IX_Media_IsMain");
            builder.HasIndex(m => m.Url).IsUnique();

        }
    }
}
