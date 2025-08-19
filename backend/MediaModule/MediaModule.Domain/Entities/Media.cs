using MediaModule.Domain.Enums;
using MediaModule.Domain.Exceptions;

namespace MediaModule.Domain.Entities
{
    public class Media
    {
        private Media(Guid entityId, string url, string name, string mediaType, EntityType entityType, bool isMain = false)
        {
            Id = Guid.NewGuid();
            EntityId = entityId;
            Url = url;
            Name = name;
            MediaType = mediaType;
            EntityType = entityType;
            IsMain = isMain;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; private set; }
        public Guid EntityId { get; private set; }
        public string Url { get; private set; } = string.Empty;
        public string Name { get; private set; }
        public bool IsMain { get; private set; } = false;
        public bool IsDeleted { get; private set; } = false;
        public string MediaType { get; private set; }
        public EntityType EntityType { get; private set; } = EntityType.Product;
        public DateTime? DeletedAt { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public static Media Create(Guid entityId, string url, string name, string mediaType, string entityType, bool isMain = false)
        {
            if (entityId == Guid.Empty)
                throw new InvalidMediaDataException("Entity ID cannot be empty.");
            if (string.IsNullOrWhiteSpace(url))
                throw new InvalidMediaDataException("URL cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidMediaDataException("Name cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(mediaType))
                throw new InvalidMediaDataException("Media type cannot be null or empty.");
            if (!Enum.TryParse<EntityType>(entityType, true, out var parsedEntityType))
                throw new InvalidMediaDataException($"Invalid entity type: {entityType}");


            return new Media(entityId, url, name, mediaType, parsedEntityType, isMain);
        }

        public void MarkAsMain()
        {
            EnsureNotDeleted();
            if (IsMain)
                throw new InvalidMediaOperationException("This media is already marked as main.");
            IsMain = true;
        }

        public void MarkAsDeleted()
        {
            EnsureNotDeleted();
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        private void EnsureNotDeleted()
        {
            if (IsDeleted)
                throw new InvalidMediaOperationException("Operation not allowed on deleted media.");
        }
    }
}
