namespace SharedKernel.Entity
{
    public abstract class Entity<TId>
    {
        public TId Id { get; protected set; }

        protected Entity()
        {
            if (typeof(TId) == typeof(Guid))
                Id = (TId)(object)Guid.NewGuid();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Entity<TId> other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public override int GetHashCode() => EqualityComparer<TId>.Default.GetHashCode(Id);
    }
}
